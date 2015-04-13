using RaftAlgorithm;
using RaftAlgorithm.States;
using RaftDemo.Model;
using RaftDemo.NodeSoftware;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace RaftDemo.ViewModels.Actors
{
    class ServerViewModel : ActorViewModel
    {
        RaftHost RaftHost;
        public ServerViewModel(RaftHost raftHost)
            : base(raftHost)
        {
            Name = raftHost.Id;
            this.RaftHost = raftHost;
            UpdateViewModel();            
            Log = new ObservableCollection<LogEntryViewModel>();
            UpdateLogViewModel();
        }
        int lastShownIndex = 0;
        void UpdateLogViewModel()
        {
            for(int i=lastShownIndex; i < RaftHost.Raft.PersistedState.LogEntries.Count; i++)
            {
                var logEntry = RaftHost.Raft.PersistedState.LogEntries[i];
                LogEntryViewModel entryViewModel = new LogEntryViewModel(logEntry);
                
                if(Log.Count >= LogVisibleEntryLimit)
                {
                    Log.RemoveAt(Log.Count - 1);
                }
                Log.Insert(0, entryViewModel);
                if (logEntry.CommitIndex < 1)
                    throw new InvalidOperationException("Log entry index must be grater than 0");
                lastShownIndex = logEntry.CommitIndex;
            }
            lastShownIndex=RaftHost.Raft.PersistedState.LogEntries.Count;
            UpdateMargins();
        }
        int LogVisibleEntryLimit = 4;

  
        void UpdateMargins()
        {
            if (Log.Count == 0)
                return;
            Log[0].CellMargin = new Thickness(0, 0, 0, 1);
            for (int i = 1; i < Log.Count-1; i++)
                Log[i].CellMargin = new Thickness(0, 0, 0, 1);
            if (Log.Count > 2)
                Log[Log.Count - 1].CellMargin = new Thickness(0, 0, 0, 0);
            for (int i=0; i < Log.Count; i++)
            {
                if (Log[i].CommitIndex <= RaftHost.Raft.CurrentIndex)
                    Log[i].EntryColor = Brushes.LightGreen;
                else
                    Log[i].EntryColor = Brushes.Yellow;
            }
        }
        public ObservableCollection<LogEntryViewModel> Log
        {
            get;
            private set;
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
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateViewModel();
                UpdateLogViewModel();
            }));
            
        }

        void UpdateViewModel()
        {
            RaftState = RaftHost.Raft.State;
            CurrentTerm = RaftHost.Raft.PersistedState.CurrentTerm;
            NodeId = RaftHost.Raft.Id;
            VotedFor = RaftHost.Raft.PersistedState.VotedFor;
            CurrentIndex = RaftHost.Raft.CurrentIndex;
        }

        private RaftNodeState _RaftState;
        public RaftNodeState RaftState
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