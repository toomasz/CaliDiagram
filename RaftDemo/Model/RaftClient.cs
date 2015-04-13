using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModel;

namespace RaftDemo.Model
{
    public class RaftClient: NodeSoftwareBase
    {
        public RaftClient(INetworkModel networkModel, string clientId) : base(networkModel)
        {
            this.Id = clientId;
            Client = networkModel.CreateClient(clientId);
            Client.ClientChannel.StateChanged += ClientChannel_StateChanged;
        }

        void ClientChannel_StateChanged(object sender, ConnectionState e)
        {
            
        }
        public INetworkClient Client
        {
            get;
            private set;
        }
        public string ServerAddress
        {
            get { return Client.RemoteAddress; }
            set { Client.RemoteAddress = value; }
        }

        protected override void OnMessageReceived(INetworkSocket channel, object message)
        {
            BroadcastExcept(message, channel);
        }
    }
}
