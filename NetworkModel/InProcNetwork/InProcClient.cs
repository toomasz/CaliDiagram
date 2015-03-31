using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    public class InProcClient: INetworkClient
    {
        public InProcClient(InProcNetwork network)
        {
            this.Network = network;
            _Channel = new InProcChannel(network, ChannelType.Client);
        }
        InProcNetwork Network;

        InProcChannel _Channel;
        public IChannel ClientChannel
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
