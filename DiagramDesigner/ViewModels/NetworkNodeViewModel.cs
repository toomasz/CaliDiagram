using DiagramLib.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System;
using System.Threading;
using DiagramDesigner.Raft;

namespace DiagramDesigner.ViewModels
{
    class PacketModel
    {
        public string Caption
        {
            get;
            set;
        }
        public NodeBaseViewModel To { get; set; }
    }
    public class NetworkNodeViewModel: NodeBaseViewModel
    {
        public NetworkSoftwareBase NodeSoftware
        {
            get;
            set;
        }
        public NetworkNodeViewModel()
        {
         
        }

        public void ButtonPressed(string name)
        {
            NodeSoftware.RaiseCommandReceived(name);
        }
        protected override void OnNameChanged(string newName)
        {
            if(NodeSoftware != null)
                NodeSoftware.Id = newName;
        }
        protected override void OnNodeCreated()
        {
            Console.WriteLine(Name + " Created");

            foreach (var connection in Connections)
            {
                NodeSoftware.RaiseChannelAdded(new NodeChannel(connection, this));
            }
            NodeSoftware.Start();
            NodeSoftware.OnMessageSent += NodeSoftware_OnMessageSent;
            NodeSoftware.Id = Name;
        }

        void NodeSoftware_OnMessageSent(object sender, OutboundMessage e)
        {
            var connection = e.DestinationChannel.Socket as ConnectionViewModel;
            connection.StartMessageAnimationFrom(this, e.Message);
        }
        protected override bool OnNodeDeleting()
        {
            NodeSoftware.OnMessageSent -= NodeSoftware_OnMessageSent;
            NodeSoftware.Stop();
            Console.WriteLine(Name + " removed");
            return true;
        }

        
        protected override void OnConnectionAdded(ConnectionViewModel connection)
        {
            var nodeChannel = new NodeChannel(connection, this);
            NodeSoftware.RaiseChannelAdded(nodeChannel);          
        }
        protected override void OnConnectionRemoved(ConnectionViewModel connection)
        {
            NodeSoftware.RaiseSocketDead(connection);
        }
    }
}
