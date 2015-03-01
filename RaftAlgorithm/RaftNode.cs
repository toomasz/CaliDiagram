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
    public class RaftNode<T>
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
        public List<LogEntry<T>> LogEntries
        {
            get;
            set;
        }

        public RaftEventResult TranslateToState(RaftNodeState newState, RaftMessageBase message = null)
        {
            RaftStateBase<T> newStateObject = null;

            if (newState == RaftNodeState.Candidate)
                newStateObject = new Candicate<T>(this);
            else if (newState == RaftNodeState.Follower)
                newStateObject = new Follower<T>(this);
            else if (newState == RaftNodeState.Leader)
                newStateObject = new Leader<T>(this);
            else
                throw new ArgumentException();
            State = newState;

            StateObject = newStateObject;
            RaftEventResult enterStateResult =  StateObject.EnterState();
            if (message != null)
            {
                if (enterStateResult.MessageToSend != null)
                    throw new Exception("WTF");
                return OnMessageReceived(message);
            }
            return enterStateResult;
        }

        public RaftNodeState State
        {
            get;
            private set;
        }

        RaftStateBase<T> StateObject
        {
            get;
            set;
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
                raftResult = StateObject.ReceiveAppendEntries(appEntries);

            /* Append entries response */
            var appEntriesResponse = raftMessage as AppendEntriesResponse;
            if (appEntriesResponse != null)
                raftResult = StateObject.ReceiveAppendEntriesResponse(appEntriesResponse);

            /* Request vote */
            var requestVote = raftMessage as RequestVote;
            if (requestVote != null)
                raftResult = StateObject.ReceiveRequestVote(requestVote);

            /* Request vote response */
            var requestVoteResponse = raftMessage as RequestVoteResponse;
            if (requestVoteResponse != null)
                raftResult = StateObject.ReceiveRequestVoteResponse(requestVoteResponse);

            if (raftResult == null)
                throw new InvalidOperationException("Raft message processing returned null");
            return raftResult;
        }

        public RaftEventResult OnTimerElapsed()
        {
            return StateObject.OnTimeout();
        }      
    }
}
