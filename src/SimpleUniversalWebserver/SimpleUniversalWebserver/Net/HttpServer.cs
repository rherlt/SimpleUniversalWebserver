using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using SimpleUniversalWebserver.Errors;
using SimpleUniversalWebserver.Extensions;
using SimpleUniversalWebserver.MessageHandler;

namespace SimpleUniversalWebserver.Net
{
    public class HttpServer : IDisposable
    {
        public int Port { get; }
        public bool ErrorWithStackTrace { get; set; } = false;
        public bool EnforceHtmlResponseBody { get; set; } = false;
        private Dictionary<int, string> _errorPages;
        public virtual Dictionary<int, string> ErrorPages
        {
            get
            {
                if (_errorPages == null)
                    _errorPages = new DefaultErrorPages().Pages;
                return _errorPages;
            }
            set { _errorPages = value; }
        }

        private const uint BufferSize = 8192;
        private StreamSocketListener _listener;
        private readonly List<IMessageHandler> _messageHandlers;
       

        public HttpServer(int port, Func<HttpRequestMessage, HttpResponseMessage, bool> messageHandler)
        {
            Port = port;
            _messageHandlers = new List<IMessageHandler>();
            _messageHandlers.Add(new SimpleMessaageHandler(messageHandler));
        }

        public HttpServer(int port, IEnumerable<IMessageHandler> messageHandlers)
        {
            Port = port;
            _messageHandlers = new List<IMessageHandler>(messageHandlers);
        }

        public virtual async Task StartServer()
        {
            try
            {
                _listener = CreateStreamSocketListener();
                await _listener.BindServiceNameAsync(Port.ToString());
            }
            catch (Exception ex)
            {
                    
                throw;
            }
        }

        public virtual async Task StopServer()
        {
            await _listener.CancelIOAsync();
            _listener.Dispose();
        }

        public virtual void Dispose()
        {
            StopServer().Wait();
        }

        protected virtual StreamSocketListener CreateStreamSocketListener()
        {
            var listener = new StreamSocketListener();
            listener.ConnectionReceived += (s, e) => ProcessRequest(s, e.Socket);
            return listener;
        }

        protected virtual async void ProcessRequest(object sender, StreamSocket socket)
        {
            Debug.WriteLine($"{DateTime.Now.ToString("s")}: request recieved from {socket.Information.RemoteAddress}:{socket.Information.RemotePort} ({socket.Information.RemoteHostName})...");

            // this works for text only

            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            
            HttpResponseMessage httpResponse = new HttpResponseMessage();
                try
                {
                    
                    var httpRequest = request.ToString().ToHttpRequest();
                    httpResponse.RequestMessage = httpRequest;
                    bool isHandled = false;
                    //iterate through all handlers to determine if message is handled
                    foreach (var handler in _messageHandlers)
                    {
                        isHandled = handler.MessageHandler(httpRequest, httpResponse);
                        if (isHandled)
                            break;
                    }

                    if (!isHandled)
                        httpResponse = CreateErrorResponse(HttpStatusCode.NotImplemented, null);

                }
                catch (NotImplementedException ex)
                {
                    httpResponse = CreateErrorResponse(HttpStatusCode.NotImplemented, ex);
                }
                catch (HttpStatusException ex)
                {
                    httpResponse = CreateErrorResponse(ex);
                }
                catch (Exception ex)
                {
                    httpResponse = CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }

                await WriteResponseAsync(httpResponse, socket.OutputStream);
                Debug.WriteLine($"{DateTime.Now.ToString("s")}: response sent...");
            
        }

        protected virtual HttpResponseMessage CreateErrorResponse(HttpStatusCode status, Exception ex)
        {
            string exceptionHtmlPartial = ErrorWithStackTrace && ex != null ? Regex.Replace(ex.ToString(), @"\r\n?|\n", "<br />") : string.Empty;

            StringContent contentHtml = ErrorPages.ContainsKey((int) status)
                ? new StringContent(string.Format(ErrorPages[(int) status], exceptionHtmlPartial), Encoding.UTF8,
                    "text/html")
                : null;

            return new HttpResponseMessage(status)
            {
                Content = contentHtml
            };
        }
        protected virtual HttpResponseMessage CreateErrorResponse(HttpStatusException ex)
        {
            return CreateErrorResponse(ex.HttpStatusCode, ex);
        }


        protected virtual HttpContent CreateHtmlResponse(HttpStatusCode status)
        {
            //check if html page for this code exists
            string content = ErrorPages.ContainsKey((int) status) ? string.Format(ErrorPages[(int)status], string.Empty) : string.Empty;
            return !string.IsNullOrEmpty(content) ? new StringContent(content, Encoding.UTF8,
                "text/html") : null;
        }

        protected virtual async Task WriteResponseAsync(HttpResponseMessage response, IOutputStream os)
        {
            if (response.Content == null && EnforceHtmlResponseBody)
                response.Content = CreateHtmlResponse(response.StatusCode);

            using (Stream resp = os.AsStreamForWrite()) {

                string header = response.ToHeaderString();
                byte[] headerArray = Encoding.UTF8.GetBytes(header);

                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                if (response.Content != null)
                    await response.Content.CopyToAsync(resp);

                await resp.FlushAsync();
            }
        }
    }
}
