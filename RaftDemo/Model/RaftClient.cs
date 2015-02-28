using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class RaftClient: NodeSoftwareBase
    {
        public RaftClient(INetworkModel networkModel, string clientId) : base(networkModel)
        {
            this.Id = clientId;
        }
        int i = 0;
        protected override void OnCommandReceived(string command)
        {
            if(command == "op")
            {
                string str = i.ToString();
                BroadcastMessage(new Message(str));
                i++;
            }
        }
        protected override void OnMessageReceived(INodeChannel channel, object message)
        {
            BroadcastExcept(message, channel);
        }
    }
}
