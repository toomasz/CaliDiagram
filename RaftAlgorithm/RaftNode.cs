using RaftAlgorithm.Messages;
using RaftAlgorithm.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            this.Id = id;

            RaftEventListener = raftEventListener;
            RaftSettings = raftSettings;
            PersistedState = new PersistedState<T>();
        }

        public PersistedState<T> PersistedState { get; private set; }

        public RaftEventResult Start()
        {
            return TranslateToState(RaftNodeState.Follower);
        }

        public string Id { get; private set; }

        public IRaftEventListener RaftEventListener
        {
            get;
            private set;
        }
        public IRaftNodeSettings RaftSettings {get; private set; }

        internal RaftEventResult TranslateToState(RaftNodeState newState, RaftMessageBase message = null)
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
            get
            {
                var stateObj = StateObject;
                if (stateObj == null)
                    return RaftNodeState.Booting;
                return stateObj.State;
            }
        }

        RaftStateBase<T> StateObject
        {
            get;
            set;
        }
        
        public int CurrentIndex
        {
            get;
            internal set;
        }


        

        public RaftEventResult OnMessageReceived(RaftMessageBase raftMessage)
        {
            RaftEventResult raftResult = null;
            /* Append entries */
            var appEntries = raftMessage as AppendEntriesRPC<T>;
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

            /* client rpc */
            var clientRequest = raftMessage as ClientRequestRPC<T>;
            if(clientRequest != null)
            {             
                PersistedState.AddEntry(clientRequest.Message);
                raftResult = RaftEventResult.Empty;
            }

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
