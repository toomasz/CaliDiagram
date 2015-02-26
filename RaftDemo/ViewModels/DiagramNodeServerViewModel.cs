using System.Collections.ObjectModel;
using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using RaftDemo.Model;
using RaftDemo.Raft.State;

namespace RaftDemo.ViewModels
{
    class DiagramNodeServerViewModel : NetworkNodeViewModel
    {
        RaftHost RaftHost;
        public DiagramNodeServerViewModel(string name, INetworkModel commModel):base(commModel)
        {
            this.Name = name;
        }
        protected override void OnNodeCreated()
        {
            base.OnNodeCreated();
            RaftHost = NodeSoftware as RaftHost;
            RaftState = RaftHost.Raft.State;
            if (RaftHost != null)
                RaftHost.OnRaftEvent += host_OnRaftEvent;
        }

        void host_OnRaftEvent(object sender, Raft.RaftEventResult e)
        {
            RaftState = RaftHost.Raft.State;
            CurrentTerm = RaftHost.Raft.CurrentTerm;
            NodeId = RaftHost.Raft.Id;
        }

        private RaftStateBase _RaftState;
        public RaftStateBase RaftState
        {
            get { return _RaftState; }
            set
            {
                if (_RaftState != value)
                {
                    _RaftState = value;
                    NotifyOfPropertyChange(() => RaftState);
                }
            }
        }

        private string _NodeId;
        public string NodeId
        {
            get { return _NodeId; }
            set
            {
                if (_NodeId != value)
                {
                    _NodeId = value;
                    NotifyOfPropertyChange(() => NodeId);
                }
            }
        }

        private int _CurrentTerm;
        public int CurrentTerm
        {
            get { return _CurrentTerm; }
            set
            {
                if (_CurrentTerm != value)
                {
                    _CurrentTerm = value;
                    NotifyOfPropertyChange(() => CurrentTerm);
                }
            }
        }
        
        
        
    }


}