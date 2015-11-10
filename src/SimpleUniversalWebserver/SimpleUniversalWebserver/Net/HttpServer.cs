using System;
using System.Collections.Generic;
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
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _messageHandler;
       

        public HttpServer(int port, Func<HttpRequestMessage, HttpResponseMessage> messageHandler)
        {
            Port = port;
            _messageHandler = messageHandler;
        }

        public virtual void StartServer()
        {
            _listener = CreateStreamSocketListener();
            _listener.BindServiceNameAsync(Port.ToString()).GetResults();
        }

        public virtual void StopServer()
        {
            _listener.Dispose();
        }

        public virtual void Dispose()
        {
            StopServer();
        }

        protected virtual StreamSocketListener CreateStreamSocketListener()
        {
            var listener = new StreamSocketListener();
            listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
            return listener;
        }

        protected virtual async void ProcessRequestAsync(StreamSocket socket)
        {
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

            using (IOutputStream output = socket.OutputStream)
            {
                HttpRequestMessage httpRequest;
                HttpResponseMessage htpResponse;
                try
                {
                    httpRequest = request.ToString().ToHttpRequest();
                    htpResponse = _messageHandler(httpRequest);
                }
                catch (NotImplementedException ex)
                {
                    htpResponse = CreateErrorResponse(HttpStatusCode.NotImplemented, ex);
                }
                catch (Exception ex)
                {
                    htpResponse = CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }

                await WriteResponseAsync(htpResponse, output);
            }
        }

        protected virtual HttpResponseMessage CreateErrorResponse(HttpStatusCode status, Exception ex)
        {
            string exceptionHtmlPartial = ErrorWithStackTrace ? Regex.Replace(ex.ToString(), @"\r\n?|\n", "<br />") : string.Empty;
            return new HttpResponseMessage(status)
            {
                Content =
                    new StringContent(string.Format(ErrorPages[(int)status], exceptionHtmlPartial), Encoding.UTF8,
                        "text/html")
            };
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

            using (Stream resp = os.AsStreamForWrite())
            {
                byte[] bodyArray = await response.Content.ReadAsByteArrayAsync();
                MemoryStream stream = new MemoryStream(bodyArray);
                string header = response.ToHeaderString();

                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
        }
    }
}
