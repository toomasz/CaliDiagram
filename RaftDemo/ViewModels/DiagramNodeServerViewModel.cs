using RaftAlgorithm;
using RaftAlgorithm.States;
using RaftDemo.Model;
using RaftDemo.NodeSoftware;

namespace RaftDemo.ViewModels
{
    class DiagramNodeServerViewModel : NetworkNodeViewModel
    {
        RaftHost RaftHost;
        public DiagramNodeServerViewModel(RaftHost raftHost)
            : base(raftHost)
        {
            Name = raftHost.Id;
            this.RaftHost = raftHost;
            UpdateViewModel();
        }
        protected override void OnNodeCreated()
        {
            base.OnNodeCreated();
            if (RaftHost != null)
                RaftHost.OnRaftEvent += host_OnRaftEvent;
            UpdateViewModel();
        }

        void host_OnRaftEvent(object sender, RaftEventResult e)
        {
            UpdateViewModel();
        }

        void UpdateViewModel()
        {
            RaftState = RaftHost.Raft.State;
            CurrentTerm = RaftHost.Raft.CurrentTerm;
            NodeId = RaftHost.Raft.Id;
            VotedFor = RaftHost.Raft.VotedFor;
            CurrentIndex = RaftHost.Raft.CurrentIndex;
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

        private string _VotedFor;
        public string VotedFor
        {
            get { return _VotedFor; }
            set
            {
                if (_VotedFor != value)
                {
                    _VotedFor = value;
                    NotifyOfPropertyChange(() => VotedFor);
                }
            }
        }

        private long _CurrentIndex;
        public long CurrentIndex
        {
            get { return _CurrentIndex; }
            set
            {
                if (_CurrentIndex != value)
                {
                    _CurrentIndex = value;
                    NotifyOfPropertyChange(() => CurrentIndex);
                }
            }
        }
        
        
        
    }


}