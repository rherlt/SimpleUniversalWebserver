using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace SimpleUniversalWebserver.MessageSerializer
{
    public interface IMessageSerializer
    {
        string Serialize(object o);
        T Deserialize<T>(string message) where T : class, new();
    }
}
