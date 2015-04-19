using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    public class InProcServer:INetworkServer
    {
        public InProcServer(InProcNetwork network)
        {
            this.Network = network;
            ClientChannels = new List<INetworkSocket>();
        }
        InProcNetwork Network;
        public bool StartListening(string address)
        {
            Address = address;
            var listeningChannel = new InProcSocket(Network, ChannelType.Listening) { LocalAddress = address };
            listeningChannel.ParentServer = this;
            Network.RegisterListeningEndpoint(address, listeningChannel);
            ListeningChannel = listeningChannel;
            return true;
        }

        public string Address
        {
            get;
            private set;
        }

        public INetworkSocket ListeningChannel
        {
            get;
            internal set;
        }

        internal void AddClientChannel(InProcSocket newClient)
        {
            ClientChannels.Add(newClient);
            newClient.ParentServer = this;
        }
        internal void RemoveClientChannel(InProcSocket exisitingClient)
        {
            exisitingClient.ParentServer = null;
            if (!ClientChannels.Remove(exisitingClient))
                throw new Exception("Failed to remove socket from server client list: " + exisitingClient.ToString());
        }

        public IList<INetworkSocket> ClientChannels
        {
            get;
            private set;
        }

        public INetworkSocket GetSocketFromBacklog()
        {
            return new InProcSocket(Network, ChannelType.Server);
        }

        public event EventHandler<INetworkSocket> ClientConnected;

        public event EventHandler<INetworkSocket> ClientDisconnected;

        internal void RaiseMessageReceived(InProcSocket socket, object message)
        {
            if (MessageReceived != null)
                MessageReceived(this, new MessageReceivedArgs(socket, message));
        }
        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public void Dispose()
        {
            Stop();
        }


        public bool Stop()
        {
            Network.UnregisterListeningEnpointFromNetwork(Address);
            ListeningChannel = null;
            
            List<INetworkSocket> sockets = new List<INetworkSocket>();
            foreach (var clientSocket in ClientChannels)
                sockets.Add(clientSocket);

            for (int i = 0; i < sockets.Count; i++)
            {
                sockets[i].Close();
            }
            return true;
        }


        
    }
}
