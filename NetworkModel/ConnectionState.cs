using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public enum ConnectionState
    {
        Closed,
        Connecting,
        ConnectionEstablished,
        ConnectionFailed,
    }
}
