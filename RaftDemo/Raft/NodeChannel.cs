using CaliDiagram.ViewModels;
using RaftDemo.ViewModels;
using System;
using System.Threading.Tasks;

namespace RaftDemo.Raft
{
    public class NodeChannel : INodeChannel
    {
        readonly ConnectionViewModel connection;

        public NodeChannel(ConnectionViewModel connection, NodeBaseViewModel from)
        {
            this.Socket = connection;
            this.connection = connection;
            this.from = from;
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
            await Task.Delay(connection.Latency);

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
    }
}
