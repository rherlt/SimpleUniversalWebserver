using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SimpleUniversalWebserver.MessageSerializer;

namespace SimpleUniversalWebserver.MessageHandler
{
    class RestMessageHandler : BaseMessageHandler
    {
        public RestMessageHandler(string baseUrl)
        {
            BaseUrl = baseUrl;
            MessageSerializer = new JsonNetMessageSerializer();
        }

        public IMessageSerializer MessageSerializer { get; set; }

        public override string BaseUrl { get; }

        public override Func<HttpRequestMessage, HttpResponseMessage, bool> MessageHandler => HandleRequest;

        protected virtual bool HandleRequest(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            return false;
        }


    }
}
