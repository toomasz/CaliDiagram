using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    class InProcSocket:INetworkSocket
    {
        public InProcSocket(InProcNetwork network, ChannelType type)
        {
            State = ConnectionState.Closed;
            Type = type;
            this.Network = network;
        }

        InProcNetwork Network;

        public string RemoteAddress
        {
            get;
            internal set;
        }

        public event EventHandler<object> MesageReceived;

        internal void ChangeStateTo(ConnectionState state)
        {
            this.State = state;
            if (StateChanged != null)
                StateChanged(this, state);
        }
        internal void RaiseMessageReceived(object message)
        {
            if (MesageReceived != null)
                MesageReceived(this, message);
            if(ParentServer != null)
            {
                ParentServer.RaiseMessageReceived(this, message);
            }
        }

        public bool SendMessage(object message)
        {
            return Network.SocketSendMessage(this, message);
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

        /// <summary>
        /// Server channel belongs to, perhaps only for listening socket
        /// </summary>
        internal InProcServer ParentServer
        {
            get;
            set;
        }

        public void Close()
        {
            Network.SocketClosingConnection(this);            
        }
        public override string ToString()
        {
            return string.Format("Local Address: {0}\nRemote Address: {1}", LocalAddress, RemoteAddress);
        }


        public event EventHandler<ConnectionState> StateChanged;
    }
}
