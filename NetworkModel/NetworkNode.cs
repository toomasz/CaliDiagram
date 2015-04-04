using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public class NetworkNode
    {
        public NetworkNode(string name, INetworkModel network)
        {
            this.Network = network;
            this.Name = name;
            Init();
        }
        void Init()
        {
            Server = Network.CreateServer(Name);
        }
        INetworkServer Server { get; set; }
        INetworkModel Network { get; set; }
        public string Name
        {
            get;
            private set;
        }
    }
}
