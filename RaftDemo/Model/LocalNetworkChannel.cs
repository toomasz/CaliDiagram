using CaliDiagram.ViewModels;
using RaftDemo.Model;
using RaftDemo.Raft;
using RaftDemo.ViewModels;
using System;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class LocalNetworkChannel : INodeChannel
    {
        readonly ConnectionViewModel connection;
        SimulationSettings worldSettings;
        public LocalNetworkChannel(ConnectionViewModel connection, NodeBaseViewModel from, SimulationSettings worldSettings, NodeChannelType channelType)
        {
            this.worldSettings = worldSettings;
            this.Socket = connection;
            this.connection = connection;
            this.from = from;
            this.ChannelType = channelType;
        }
        NodeBaseViewModel from;
        public void SendMessage(object message)
        {
            SendMessageAsync(message);
        }

        public async Task SendMessageAsync(object message)
        {

            NetworkNodeViewModel to = null;
           
            if (connection.From == from)
                to = (NetworkNodeViewModel)connection.To;
            else if (connection.To == from)
                to = (NetworkNodeViewModel)connection.From;
            else
                throw new ArgumentException();

            // this part should happen on network link
            await Task.Delay(Convert.ToInt32(connection.Latency));

            // data arrived and is assembled into packet
            INodeChannel messageChannel = to.NodeSoftware.GetChannelBySocket(connection);
            
            if (messageChannel == null)
                return;

            to.NodeSoftware.RaisePacketReceived(message, messageChannel);
        }

        public object Socket
        {
            get;
            private set;
        }


        public NodeChannelType ChannelType
        {
            get;
            private set;
        }
    }
}
