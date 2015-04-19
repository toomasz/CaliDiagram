using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors
{
    public class ActorChannel
    {
        public ActorChannel(INetworkSocket Socket)
        {
            this.Socket = Socket;
        }
        public INetworkSocket Socket { get; private set; }
    }
}
