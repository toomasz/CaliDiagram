using CaliDiagram.ViewModels;
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
using RaftDemo.Raft;

namespace RaftDemo.ViewModels
{
    public class NetworkNodeViewModel: NodeBaseViewModel
    {
        public NetworkSoftwareBase NodeSoftware
        {
            get;
            set;
        }
        public NetworkNodeViewModel()
        {
            StartText = "Pause";
        }

        public void ButtonPressed(string name)
        {
            if(name == "startStop")
            {
                ButtonStartStopClick();
                return;
            }
            NodeSoftware.RaiseCommandReceived(name);
        }
        protected override void OnNameChanged(string newName)
        {
            if(NodeSoftware != null)
                NodeSoftware.Id = newName;
        }
        bool wasCreated = false;
        protected override void OnNodeCreated()
        {
            if (wasCreated)
                throw new Exception("NetworkNodeViewModel::OnNodeCreated called twice");
            wasCreated = true;
            Console.WriteLine(Name + " Created");

            foreach (var connection in Connections)
            {
                NodeSoftware.RaiseChannelAdded(new NodeChannel(connection, this));
            }
            NodeSoftware.Start();
            NodeSoftware.OnMessageSent += NodeSoftware_OnMessageSent;
            NodeSoftware.Id = Name;
        }

        public void ButtonStartStopClick()
        {
            if (NodeSoftware.IsStarted)
            {
                NodeSoftware.Stop();
                StartText = "Resume";
            }
            else
            {
                NodeSoftware.Start();
                StartText = "Pause";
            }
        } 
        
        private string _StartText;
        public string StartText
        {
            get { return _StartText; }
            set
            {
                if (_StartText != value)
                {
                    _StartText = value;
                    NotifyOfPropertyChange(() => StartText);
                }
            }
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
