using DiagramDesigner.ViewModels;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
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
            INodeChannel messageChannel = to.NodeSoftware.Channels.FirstOrDefault(chann => chann.Socket == connection);
            
            if (messageChannel == null)
                return;
            InboundMessage messageObject = new InboundMessage() { Message = message, SourceChannel = messageChannel };

            to.NodeSoftware.InputQueue.Add(messageObject);
        }

        public object Socket
        {
            get;
            private set;
        }
    }
}
