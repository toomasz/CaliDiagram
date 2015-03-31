using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    class InProcChannel:IChannel
    {
        public InProcChannel(InProcNetwork network, ChannelType type)
        {
            State = ConnectionState.Closed;
            Type = type;
        }
        public string RemoteAddress
        {
            get;
            internal set;
        }

        public event EventHandler<object> MesageReceived;
        internal void ChangeStateTo(ConnectionState state)
        {
            this.State = state;
        }
        public bool SendMessage(object message)
        {
            return true;
        }

        public ChannelType Type
        {
            get;
            private set;
        }

        public ConnectionState State
        {
            get;
            private set;
        }


        public string LocalAddress
        {
            get;
            internal set;
        }
    }
}
