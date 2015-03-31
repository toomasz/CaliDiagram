using RaftAlgorithm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.States
{
    public class Leader<T> : RaftStateBase<T>
    {
        public Leader(RaftNode<T> node):base(node)
        {
            
        }
        public override RaftNodeState State
        {
            get { return RaftNodeState.Leader; }
        }
        /// <summary>
        /// Index of highest log entry known to be commited(initialized to 0, increases monotonically) 
        /// </summary>
        readonly Dictionary<string, int> NextIndex = new Dictionary<string, int>();
        /// <summary>
        /// For each server, index of highest log entry applied to state machine(initialized to 0, increases monotonically)
        /// </summary>
        readonly Dictionary<string, int> MatchIndex = new Dictionary<string, int>();


        public override RaftEventResult EnterState()
        {
            return BroadcastAppendEntries();
        }

        RaftEventResult BroadcastAppendEntries()
        {
            Node.RaftEventListener.OnAppendEntries();            
         
            var appendEntriesMessage = new AppendEntriesRPC<T>(Node.PersistedState.CurrentTerm, Node.Id);
            appendEntriesMessage.LeaderCommit = Node.CurrentIndex;
            //for (int i=Node.CurrentIndex; i < Node.Log.Count; i++)
            //{
            //    if (appendEntriesMessage.LogEntries == null)
            //        appendEntriesMessage.LogEntries = new List<LogEntry<T>>();
            //    appendEntriesMessage.LogEntries.Add(Node.Log[i]);
            //}
            return RaftEventResult.BroadcastMessage(appendEntriesMessage).SetTimer(Node.RaftSettings.LeaderTimeoutFrom, Node.RaftSettings.LeaderTimeoutTo);
        }

        public override RaftEventResult OnTimeout()
        {
            return BroadcastAppendEntries();   
        }

        public override RaftEventResult ReceiveRequestVote(RequestVote requestVote)
        {
            if(requestVote.CandidateTerm > CurrentTerm)
            {
                return Node.TranslateToState(RaftNodeState.Follower, requestVote);
            }
            return RaftEventResult.ReplyMessage(DenyVote);
        }

        public override RaftEventResult ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse)
        {
            // got vote but we dont care because we are leader already
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveAppendEntries(AppendEntriesRPC<T> appendEntries)
        {
            if (appendEntries.LeaderTerm > CurrentTerm)
            {
                return Node.TranslateToState(RaftNodeState.Follower, appendEntries);
            }
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse)
        {
            if (appendEntriesResponse.FollowerTerm > CurrentTerm)
            {
                return Node.TranslateToState(RaftNodeState.Follower);
            }
            return RaftEventResult.Empty;
        }
        public override string ToString()
        {
            return "Leader";
        }
    }
}
