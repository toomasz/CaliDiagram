using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaftDemo.NodeSoftware;

namespace RaftDemo.Model
{
    public class InProcNetworkNode : INetworkNode
    {
        public InProcNetworkNode(string address)
        {
            this.Address = address;
        }
        public string Address
        {
            get;
            private set;
        }

        public void RequestConnectionTo(string destinationAddress)
        {
            throw new NotImplementedException();
        }
    }
}
