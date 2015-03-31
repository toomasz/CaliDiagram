using System;
using System.Collections.Generic;
using CaliDiagram.ViewModels;
using RaftDemo.NodeSoftware;
using RaftDemo.ViewModels;

namespace RaftDemo.Model
{
    public class InProcNetwork: INetworkModel
    {
        SimulationSettings worldSettings;
        public InProcNetwork(SimulationSettings worldSettings)
        {
            this.worldSettings = worldSettings;
        }
        public INodeChannel CreateChannel(ConnectionViewModel connection, NodeBaseViewModel from)
        {
            NodeChannelType channelType = NodeChannelType.ClientToServer;
            if (connection is ServerToServerConnectionViewModel)
                channelType = NodeChannelType.ServerToServer;
            return new InProcNetworkChannel(connection, from, worldSettings, channelType);
        }
        List<INetworkNode> NetworkNodes = new List<INetworkNode>();

        public INetworkNode CreateListener(string address)
        {
            return new InProcNetworkNode(address); 
        }
    }
}
