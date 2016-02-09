using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SimpleUniversalWebserver.MessageSerializer;
using HttpMethod = Windows.Web.Http.HttpMethod;

namespace SimpleUniversalWebserver.MessageHandler
{
    public abstract class RestController : BaseMessageHandler
    {
        protected RestController(string baseUrl)
        {
            BaseUrl = baseUrl.ToLower();
            MessageSerializer = new JsonNetMessageSerializer();
        }

        public IMessageSerializer MessageSerializer { get; set; }

        public override string BaseUrl { get; }

        public override Func<HttpRequestMessage, HttpResponseMessage, bool> MessageHandler => HandleRequest;

        protected virtual bool HandleRequest(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            if (BaseUrl == httpRequestMessage.RequestUri.OriginalString.ToLower())
            {
                switch (httpRequestMessage.Method.Method.ToLower())
                {
                    case "get":
                        Get(httpRequestMessage, httpResponseMessage);
                        return true;
                    case "post":
                        Post(httpRequestMessage, httpResponseMessage);
                        return true;
                    case "put":
                        Put(httpRequestMessage, httpResponseMessage);
                        return true;
                    case "delete":
                        Delete(httpRequestMessage, httpResponseMessage);
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        protected virtual void Get(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            NotImplemeted(httpRequestMessage, httpResponseMessage);
        }

        protected virtual void Post(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            NotImplemeted(httpRequestMessage, httpResponseMessage);
        }

        protected virtual void Put(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            NotImplemeted(httpRequestMessage, httpResponseMessage);
        }

        protected virtual void Delete(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            NotImplemeted(httpRequestMessage, httpResponseMessage);
        }

        protected void NotImplemeted(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.StatusCode = HttpStatusCode.NotImplemented;
            httpResponseMessage.Content = new StringContent($"The Method {httpRequestMessage.Method} method is not implemented!");
        }
    }
}
