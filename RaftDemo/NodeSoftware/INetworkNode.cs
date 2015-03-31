using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.NodeSoftware
{
    public interface INetworkNode
    {
        string Address { get; }
        void RequestConnectionTo(string destinationAddress);
    }
}
