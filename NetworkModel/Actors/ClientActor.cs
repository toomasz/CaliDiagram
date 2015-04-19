using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors
{
    /// <summary>
    /// Client actor can only make connection to server actors
    /// </summary>
    public class ClientActor : ActorBase
    {
        public ClientActor(INetworkModel networkModel)
            :base(networkModel)
        {

        }
    }
}
