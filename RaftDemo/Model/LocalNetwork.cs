using CaliDiagram.ViewModels;
using RaftDemo.Raft;
using RaftDemo.ViewModels;

namespace RaftDemo.Model
{
    public class LocalNetwork: INetworkModel
    {
        SimulationSettings worldSettings;
        public LocalNetwork(SimulationSettings worldSettings)
        {
            this.worldSettings = worldSettings;
        }
        public INodeChannel CreateChannel(ConnectionViewModel connection, NodeBaseViewModel from)
        {
            NodeChannelType channelType = NodeChannelType.ClientToServer;
            if (connection is ServerToServerConnectionViewModel)
                channelType = NodeChannelType.ServerToServer;
            return new LocalNetworkChannel(connection, from, worldSettings, channelType);
        }
    }
}
