using RaftAlgorithm.Messages;
using RaftAlgorithm.States;
using System;
using System.Collections.Generic;

/*
 * notes - election timeout 150 - 300 ms  
 * broadcastTime ≪ electionTimeout ≪ MTBF
 */

namespace RaftAlgorithm
{
    public class RaftNode
    {
        public RaftNode(IRaftEventListener raftEventListener, IRaftNodeSettings raftSettings, string id)
        {
            if (raftEventListener == null)
                throw new ArgumentNullException("raftWorld");
            if (id == null)
                throw new ArgumentException("id");
            this._id = id;

            RaftEventListener = raftEventListener;
            RaftSettings = raftSettings;
            CurrentTerm = 0;
            LogEntries = new List<LogEntry>();
            LogEntries.Add(new LogEntry() { Data = "A=2", CommitIndex = 1, Term = 1 });
            LogEntries.Add(new LogEntry() { Data = "C=1", CommitIndex = 2, Term = 1 });
            
        }
        private readonly string _id;
        public string Id
        {
            get
            {
                return _id;
            }
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
            RaftEventResult enterStateResult =  State.EnterState();
            if (message != null)
            {
                if (enterStateResult.MessageToSend != null)
                    throw new Exception("WTF");
                return OnMessageReceived(message);
            }
            return enterStateResult;
        }

        private RaftStateBase _State;
        public RaftStateBase State
        {
            get;
            internal set;
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
                }
            }
        }

        public int CurrentIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Candiate Id that received vote in current term
        /// </summary>
        private string _VotedFor;
        public string VotedFor
        {
            get;
            internal set;
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
