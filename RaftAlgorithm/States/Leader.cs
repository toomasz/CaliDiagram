using RaftAlgorithm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.States
{
    public class Leader : RaftStateBase
    {
        public Leader(RaftNode node):base(node)
        {
            
        }
        public override string ToString()
        {
            return "Leader";
        }

        public override RaftEventResult EnterState()
        {
            return BroadcastAppendEntries();
        }

        RaftEventResult BroadcastAppendEntries()
        {
            Node.RaftEventListener.OnAppendEntries();            
          //  Node.RaftTimer.SetRandomTimeout(Node.RaftSettings.LeaderTimeoutFrom,Node.RaftSettings.LeaderTimeoutTo);

            var appendEntriesMessage = new AppendEntries(Node.CurrentTerm, Node.Id);
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

        public override RaftEventResult ReceiveAppendEntries(AppendEntries appendEntries)
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
    }
}
