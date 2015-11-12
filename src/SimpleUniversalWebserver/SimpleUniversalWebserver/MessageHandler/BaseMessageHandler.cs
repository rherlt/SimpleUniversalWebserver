using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUniversalWebserver.MessageHandler
{
    public abstract class BaseMessageHandler : IMessageHandler
    {
        public abstract string BaseUrl { get; }
        public abstract Func<HttpRequestMessage, HttpResponseMessage, bool> MessageHandler { get; }
    }
}
