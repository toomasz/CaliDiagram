using DiagramDesigner.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.State
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
        public override void EnterState()
        {
            // Start new election(translate to candidate) of no sign of leader for random time specified here
            Node.RaftTimer.SetRandomTimeout(200, 400);
        }
        public override void ExitState()
        {

        }

        public override void OnTimeout()
        {
            // translate to candidate and start new election
            Node.TranslateToState(RaftNodeState.Candidate);
        }
        
        public override void ReceiveRequestVote(RequestVote requestVote, INodeChannel sourceChannel)
        {
            // we have more recent term so we dont vote
            if(requestVote.CandidateTerm < requestVote.CandidateTerm)
            {
                Node.SendMessage(sourceChannel, DenyVote);
                return;
            }

            if(requestVote.CandidateTerm > CurrentTerm)
            {
                CurrentTerm = requestVote.CandidateTerm;

            }
            // if haven't voted before
            if (Node.VotedFor == null || Node.VotedFor == requestVote.CandidateId)
            {
                Node.SendMessage(sourceChannel, GrantVote);
                Node.VotedFor = requestVote.CandidateId;
                Node.RaftTimer.SetRandomTimeout(4000,4000);
                return;
            }
        }

        public override void ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse, INodeChannel sourceChannel)
        {

        }

        public override void ReceiveAppendEntries(AppendEntries appendEntries, INodeChannel sourceChannel)
        {
            if (appendEntries.LeaderTerm < Node.CurrentTerm)
                return;

            CurrentTerm = appendEntries.LeaderTerm;
            Node.RaftTimer.SetRandomTimeout(200, 400);            
        }

        public override void ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse, INodeChannel sourceChannel)
        {
            
        }
    }
}
