using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModel;
using NetworkModel.Actors;

namespace ActorTest
{
    public class TestClientActor: ClientActor
    {
        public TestClientActor(INetworkModel networkModel) : base(networkModel)
        {
        }
    }
}
