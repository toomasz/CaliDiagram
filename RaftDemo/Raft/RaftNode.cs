using RaftDemo.Raft.Messages;
using RaftDemo.Raft.State;
using System;
using System.Collections.Generic;


/*
 * notes - election timeout 150 - 300 ms  
 * broadcastTime ≪ electionTimeout ≪ MTBF
 */

namespace RaftDemo.Raft
{
    public class RaftNode
    {
        public RaftNode(IRaftEventListener raftEventListener, IRaftNodeSettings raftSettings, string Id)
        {
            if (raftEventListener == null)
                throw new ArgumentNullException("raftWorld");
            RaftEventListener = raftEventListener;
            RaftSettings = raftSettings;
            CurrentTerm = 0;
            LogEntries = new List<LogEntry>();
            LogEntries.Add(new LogEntry() { Data = "A=2", CommitIndex = 1, Term = 1 });
            LogEntries.Add(new LogEntry() { Data = "C=1", CommitIndex = 2, Term = 1 });
            this.Id = Id;
        }

        public string Id
        {
            get;
            private set;
        }

        public IRaftEventListener RaftEventListener
        {
            get;
            private set;
        }
        public IRaftNodeSettings RaftSettings
        {
            get;
            private set;
        }
        public List<LogEntry> LogEntries
        {
            get;
            set;
        }

        public RaftEventResult TranslateToState(RaftNodeState newState, RaftMessageBase message = null)
        {
            RaftStateBase newStateObject = null;

            if (newState == RaftNodeState.Candidate)
                newStateObject = new Candicate(this);
            else if (newState == RaftNodeState.Follower)
                newStateObject = new Follower(this);
            else if (newState == RaftNodeState.Leader)
                newStateObject = new Leader(this);
            else
                throw new ArgumentException();
           
            State = newStateObject;
            RaftEventResult result1 =  State.EnterState();
            if (message != null)
            {
                if (result1.MessageToSend != null)
                    throw new Exception("WTF");
                return OnMessageReceived(message);
            }
            return result1;
        }

        private RaftStateBase _State;
        public RaftStateBase State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                   // NotifyOfPropertyChange(() => State);
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
                    VotedFor = null;
                  //  NotifyOfPropertyChange(() => CurrentTerm);
                }
            }
        }

        /// <summary>
        /// Candiate Id that received vote in current term
        /// </summary>
        private string _VotedFor;
        public string VotedFor
        {
            get { return _VotedFor; }
            set
            {
                if (_VotedFor != value)
                {
                    _VotedFor = value;
                 //   NotifyOfPropertyChange(() => VotedFor);
                }
            }
        }
        
        public RaftEventResult OnMessageReceived(RaftMessageBase raftMessage)
        {
            RaftEventResult raftResult = null;
            /* Append entries */
            var appEntries = raftMessage as AppendEntries;
            if (appEntries != null)
                raftResult = State.ReceiveAppendEntries(appEntries);

            /* Append entries response */
            var appEntriesResponse = raftMessage as AppendEntriesResponse;
            if (appEntriesResponse != null)
                raftResult = State.ReceiveAppendEntriesResponse(appEntriesResponse);

            /* Request vote */
            var requestVote = raftMessage as RequestVote;
            if (requestVote != null)
                raftResult = State.ReceiveRequestVote(requestVote);

            /* Request vote response */
            var requestVoteResponse = raftMessage as RequestVoteResponse;
            if (requestVoteResponse != null)
                raftResult = State.ReceiveRequestVoteResponse(requestVoteResponse);

            if (raftResult == null)
                throw new InvalidOperationException("Raft message processing returned null");
            return raftResult;
        }

        public RaftEventResult OnTimerElapsed()
        {
            return State.OnTimeout();
        }      
    }
}
