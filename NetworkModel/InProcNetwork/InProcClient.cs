using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    public class InProcClient: INetworkClient
    {

        public InProcClient(InProcNetwork network,string address = null)
        {
            this.Network = network;
            if (address == null)
                address = Network.GetNextClientSocketAddress();

            _Channel = new InProcSocket(network, ChannelType.Client) { LocalAddress = address };
        }
        InProcNetwork Network;

        InProcSocket _Channel;
        public INetworkSocket ClientChannel
        {
            get
            {
                return _Channel;
            }
        }

        public void RequestConnectionTo(string remoteAddress)
        {
            Network.RequestClientConnectioTo(_Channel, remoteAddress);
        }

        public void Close()
        {
            ClientChannel.Close();
        }

        public string LocalAddress
        {
            get
            {
                return ClientChannel.LocalAddress;
            }
        }

        public string RemoteAddress
        {
            get
            {
                return _Channel.RemoteAddress;
            }
        }
    }
}
