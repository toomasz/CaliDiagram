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
            ClientChannels = new List<IChannel>();
        }
        InProcNetwork Network;
        public bool StartListening(string address)
        {
            Network.RegisterServerEndpoint(address, this);
            ListeningChannel = new InProcChannel(Network, ChannelType.Server) { LocalAddress = address };
            return true;
        }
        public IChannel ListeningChannel
        {
            get;
            internal set;
        }

        public void AddClientChannel(IChannel newClient)
        {
            ClientChannels.Add(newClient);
        }

        public IList<IChannel> ClientChannels
        {
            get;
            private set;
        }

        public IChannel GetSocketFromBacklog()
        {
            return new InProcChannel(Network, ChannelType.Server);
        }

        public event EventHandler<IChannel> NewClientChannel;

        public event EventHandler<IChannel> ClientChannelClosed;
    }
}
