using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUniversalWebserver.MessageHandler
{
    public interface IMessageHandler
    {
        Func<HttpRequestMessage, HttpResponseMessage, bool> MessageHandler { get; }
    }
}
