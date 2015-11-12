using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUniversalWebserver.MessageHandler
{
    public class SimpleMessaageHandler : IMessageHandler
    {
        public SimpleMessaageHandler(Func<HttpRequestMessage, HttpResponseMessage, bool> messageHandler)
        {
            MessageHandler = messageHandler;
        }

        public Func<HttpRequestMessage, HttpResponseMessage, bool> MessageHandler { get; }
    }
}
