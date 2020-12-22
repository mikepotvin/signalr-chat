using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChat.ConsoleApp
{
    public enum ConnectionState
    {
        Connecting,
        Connected,
        Reconnecting,
        Disconnected
    }
}
