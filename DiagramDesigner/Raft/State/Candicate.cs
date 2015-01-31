using DiagramDesigner.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.State
{
    public class Candicate : RaftStateBase
    {
        public Candicate(RaftNode node)
            : base(node)
        {
            
        }

        int NodeCount = 4;

        public override string ToString()
        {
            return "Candidate";
        }
        static Random rnd = new Random();
        public override void EnterState()
        {
            
            StartNewElection();
        }

        void StartNewElection()
        {
            VoteCount = 1;
            Node.CurrentTerm++;
            Node.RaftTimer.SetTimeout(1000 + rnd.Next(2700));
            Node.BroadcastMessage(new RequestVote() { CandidateId = Node.Id, CandidateTerm = Node.CurrentTerm });
        }

        int VoteCount = 1;
        public override void ReceiveMessage(RaftMessageBase message, INodeChannel channel)
        {
            if(message is RequestVoteResult)
            {
                RequestVoteResult result = message as RequestVoteResult;
                if (result.VoteGranted)
                {
                    VoteCount++;
                    Node.CurrentTerm = result.CurrentTerm;
                }
                if (VoteCount > NodeCount / 2)
                    Node.TranslateToState( RaftNodeState.Leader);
            }
            if (message is RequestVote)
            {
                //var requestVote = message as RequestVote;
                //bool voteGranted = true;
                //if (requestVote.CandidateTerm < Node.CurrentTerm)
                //    voteGranted = false;
                //Node.SendMessage(channel, new RequestVoteResult() { VoteGranted = voteGranted, CurrentTerm = Node.CurrentTerm });
            }

            if (message is AppendEntries)
                Node.TranslateToState(RaftNodeState.Follower);
        }
        public override void OnTimeout()
        {
            StartNewElection();
        }
        public override void ExitState()
        {

        }
    }
}
