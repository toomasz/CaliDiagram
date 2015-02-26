using RaftAlgorithm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.States
{
    public class Follower : RaftStateBase
    {
        public Follower(RaftNode node)
            : base(node)
        {
            
        }
        public override string ToString()
        {
            return "Follower";
            
        }
        static Random rnd = new Random();
        public override RaftEventResult EnterState()
        {
            // Start new election(translate to candidate) of no sign of leader for random time specified here
           // Node.RaftTimer.SetRandomTimeout(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
            return RaftEventResult.Empty.SetTimer(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
        }

        public override RaftEventResult OnTimeout()
        {
            // translate to candidate and start new election
            return Node.TranslateToState(RaftNodeState.Candidate);
        }

        public override RaftEventResult ReceiveRequestVote(RequestVote requestVote)
        {
            // we have more recent term so we dont vote
            if(requestVote.CandidateTerm < CurrentTerm)
            {
                return RaftEventResult.ReplyMessage(DenyVote);
            }
 
            if(requestVote.CandidateTerm > CurrentTerm)
            {
                CurrentTerm = requestVote.CandidateTerm;
            }
            // if haven't voted before
            if (Node.VotedFor == null || Node.VotedFor == requestVote.CandidateId)
            {
                Node.VotedFor = requestVote.CandidateId;
              //  Node.RaftTimer.SetRandomTimeout(Node.RaftSettings.FollowerTimeoutFrom*2, Node.RaftSettings.FollowerTimeoutTo*2);
                return RaftEventResult.ReplyMessage(GrantVote).SetTimer(Node.RaftSettings.FollowerTimeoutFrom * 2, Node.RaftSettings.FollowerTimeoutTo * 2);
            }
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse)
        {
            return RaftEventResult.Empty;
        }

        public override RaftEventResult ReceiveAppendEntries(AppendEntries appendEntries)
        {
            if (appendEntries.LeaderTerm < Node.CurrentTerm)
                return RaftEventResult.Empty;

            CurrentTerm = appendEntries.LeaderTerm;
           // Node.RaftTimer.SetRandomTimeout(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
            return RaftEventResult.Empty.SetTimer(Node.RaftSettings.FollowerTimeoutFrom, Node.RaftSettings.FollowerTimeoutTo);
        }

        public override RaftEventResult ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse)
        {
            return RaftEventResult.Empty;
        }
    }
}
