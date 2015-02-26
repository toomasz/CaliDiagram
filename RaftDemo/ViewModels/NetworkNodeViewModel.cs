using CaliDiagram.ViewModels;
using RaftDemo.Model;
using RaftDemo.Raft;
using System;

namespace RaftDemo.ViewModels
{
    public class NetworkNodeViewModel: NodeBaseViewModel
    {
        INetworkModel commModel;
        public NetworkNodeViewModel(INetworkModel commModel)
        {
            this.commModel = commModel;
            StartText = "Pause";
        }

        public NetworkSoftwareBase NodeSoftware
        {
            get;
            set;
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
                NodeSoftware.RaiseChannelAdded(commModel.CreateChannel(connection, this));
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
            INodeChannel newChannel= commModel.CreateChannel(connection, this);
          
            NodeSoftware.RaiseChannelAdded(newChannel);          
        }
        protected override void OnConnectionRemoved(ConnectionViewModel connection)
        {
            NodeSoftware.RaiseSocketDead(connection);
        }
    }
}
