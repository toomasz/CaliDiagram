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
        uint clientSequence = 0;
        protected override void OnCommandReceived(string command)
        {
            if(command == "op")
            {
                string str = string.Format("c_{0}:{1}", Id, clientSequence);
                BroadcastMessage(new Message(str));
                clientSequence++;
            }
        }
        protected override void OnMessageReceived(INodeChannel channel, object message)
        {
            BroadcastExcept(message, channel);
        }
    }
}
