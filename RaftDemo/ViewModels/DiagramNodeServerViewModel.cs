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
            RaftHost.Raft.Log.CollectionChanged += Log_CollectionChanged;
            Log = new ObservableCollection<LogEntryViewModel>();
         
        }
        int LogVisibleEntryLimit = 4;

        void Log_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var logEntry = e.NewItems[0] as LogEntry<string>;
                var logEntryViewModel = new LogEntryViewModel(logEntry);
                Log.Insert(0, logEntryViewModel);
                if (Log.Count > LogVisibleEntryLimit)
                    Log.RemoveAt(Log.Count - 1);
                UpdateMargins();
            }));
        }
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