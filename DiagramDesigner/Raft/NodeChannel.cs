using DiagramDesigner.ViewModels;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public class NodeChannel : INodeChannel
    {
        ConnectionViewModel connection;
        public NodeChannel(ConnectionViewModel connection, NodeBaseViewModel from)
        {
            this.Socket = connection;
            this.connection = connection;
            this.from = from;
        }
        NodeBaseViewModel from;
        public void SendMessage(object message)
        {
            SendMessage(message, 440);
        }
        public async Task SendMessage(object message, int delay)
        {

            NetworkNodeViewModel to = null;
           
            if (connection.From == from)
                to = (NetworkNodeViewModel)connection.To;
            else if (connection.To == from)
                to = (NetworkNodeViewModel)connection.From;
            else
                throw new ArgumentException();

            //PacketModel packet = new PacketModel() { Caption = message.ToString(), To = to };
           // if (PacketSent != null)
           //     PacketSent(this, packet);

            await Task.Delay(connection.Latency);
            INodeChannel messageChannel = to.NodeSoftware.Channels.FirstOrDefault(chann => chann.Socket == connection);
            
            if (messageChannel == null)
                return;
            InboundMessage messageObject = new InboundMessage() { Message = message, SourceChannel = messageChannel };

            to.NodeSoftware.InputQueue.Add(messageObject);
            // to.OnMessageReceived(connection, message);
        }

        public object Socket
        {
            get;
            private set;
        }
    }
}
