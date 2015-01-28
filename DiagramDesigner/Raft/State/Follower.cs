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
            Node.RaftTimer.SetTimeout(3000 + rnd.Next(5000));
        }
        public override void ExitState()
        {

        }
        public override void ReceiveMessage(RaftMessageBase message, INodeChannel channel)
        {
            if (message is AppendEntries)
            {
                Node.TranslateToState(RaftNodeState.Follower);
            }
            if (message is RequestVote)
            {
                Node.TranslateToState(RaftNodeState.Follower);
                Node.SendMessage(channel, new RequestVoteResult() { VoteGranted = true });
            }
        }
        public override void OnTimeout()
        {
            Node.TranslateToState(RaftNodeState.Candidate);
        }
    }
}
