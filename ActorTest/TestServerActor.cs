using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModel;
using NetworkModel.Actors;

namespace ActorTest
{
    public class TestServerActor: ServerActor
    {
        public TestServerActor(INetworkModel networkModel, string address) : base(networkModel, address)
        {
        }

        protected override void OnMessageReceived(ActorChannel channel, object message)
        {
            
        }
    }
}
