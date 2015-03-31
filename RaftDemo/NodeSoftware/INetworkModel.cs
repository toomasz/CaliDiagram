using CaliDiagram.ViewModels;
using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.NodeSoftware
{
    public interface INetworkModel
    {
        INetworkNode CreateListener(string address);

        INodeChannel CreateChannel(ConnectionViewModel connection, NodeBaseViewModel from);
       
        //IAddress LookupName(string name)
        //
    }
}
