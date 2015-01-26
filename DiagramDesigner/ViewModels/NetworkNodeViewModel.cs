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
        public NetworkNode NodeSoftware
        {
            get;
            set;
        }
        public NetworkNodeViewModel()
        {
          
        }

        public int GetDelay(ConnectionViewModel connection)
        {
            return 20;
        }


        public event EventHandler<object> PacketSent;



        void ClockElapsed()
        {
            
        }
    

        public void ButtonPressed(string name)
        {
            NodeSoftware.ApplyCommand(name);
        }

        protected override void OnNodeCreated()
        {
            Console.WriteLine(Name + " Created");

            foreach (var connection in Connections)
            {
                NodeSoftware.Channels.Add(new NodeChannel(connection, this));
            }
            NodeSoftware.Start();
            NodeSoftware.OnMessageSent += NodeSoftware_OnMessageSent;
        }

        void NodeSoftware_OnMessageSent(object sender, OutboundMessage e)
        {
            var connection = e.DestinationChannel.Socket as ConnectionViewModel;
            connection.SendMessageFrom(this, e.Message);
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
            NodeSoftware.Channels.Add(new NodeChannel(connection, this));
        }
        protected override void OnConnectionRemoved(ConnectionViewModel connection)
        {
            
        }
    }
}
