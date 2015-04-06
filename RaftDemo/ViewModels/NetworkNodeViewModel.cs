using CaliDiagram.ViewModels;
using RaftDemo.Model;
using RaftDemo.NodeSoftware;
using System;
using NetworkModel;

namespace RaftDemo.ViewModels
{
    public class NetworkNodeViewModel: NodeBaseViewModel
    {
        public NetworkNodeViewModel(NodeSoftwareBase nodeSoftware)
        {
            StartText = "Pause";
            this.NodeSoftware = nodeSoftware;
            NodeSoftware.IsStartedChanged += NodeSoftware_IsStartedChanged;
        }

        void NodeSoftware_IsStartedChanged(object sender, bool e)
        {
            IsStarted = e;
        }

        private bool _IsStarted;
        public bool IsStarted
        {
            get { return _IsStarted; }
            set
            {
                if (_IsStarted != value)
                {
                    _IsStarted = value;
                    NotifyOfPropertyChange(() => IsStarted);
                }
            }
        }
        

        public NodeSoftwareBase NodeSoftware
        {
            get;
            private set;
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
               // NodeSoftware.RaiseChannelAdded(NodeSoftware.NetworkModel.CreateChannel(connection, this));
            }
            NodeSoftware.Start();
            NodeSoftware.OnMessageSent += NodeSoftware_OnMessageSent;
            NodeSoftware.Id = Name;
        }

        protected override bool OnNodeDeleting()
        {
            NodeSoftware.OnMessageSent -= NodeSoftware_OnMessageSent;
            NodeSoftware.IsStartedChanged -= NodeSoftware_IsStartedChanged;
            NodeSoftware.Stop();
            Console.WriteLine(Name + " removed");
            return true;
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
          //  var connection = e.DestinationChannel.Socket as ConnectionViewModel;
          //  connection.StartMessageAnimationFrom(this, e.Message);
        }
        
        
        protected override void OnConnectionAdded(ConnectionViewModel connection)
        {
           // INetworkSocket newChannel = NodeSoftware.NetworkModel.CreateChannel(connection, this);
          
          //  NodeSoftware.RaiseChannelAdded(newChannel);          
        }
        protected override void OnConnectionRemoved(ConnectionViewModel connection)
        {
            NodeSoftware.RaiseSocketDead(connection);
        }
        
    }
}
