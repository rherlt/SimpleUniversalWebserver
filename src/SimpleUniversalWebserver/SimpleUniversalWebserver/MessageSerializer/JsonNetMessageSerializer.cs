using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleUniversalWebserver.MessageSerializer
{
    public class JsonNetMessageSerializer : IMessageSerializer
    {
        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public T Deserialize<T>(string message) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
