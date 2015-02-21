using RaftDemo.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft.State
{
    class Leader : RaftStateBase
    {
        public Leader(RaftNode node):base(node)
        {
            
        }
        public override string ToString()
        {
            return "Leader";
        }
        Random rnd = new Random();
        public override void EnterState()
        {
            BroadcastAppendEntries();
        }

        void BroadcastAppendEntries()
        {
            Node.RaftWorld.OnAppendEntries();
            Node.BroadcastMessage(new AppendEntries(Node.CurrentTerm, Node.Id));
            Node.RaftTimer.SetRandomTimeout(500,500);
        }

        public override void OnTimeout()
        {
            BroadcastAppendEntries();   
        }
        public override void ExitState()
        {

        }

        public override void ReceiveRequestVote(RequestVote requestVote, INodeChannel sourceChannel)
        {
            if(requestVote.CandidateTerm > CurrentTerm)
            {
                Node.TranslateToState(RaftNodeState.Follower);
                Node.RaisePacketReceived(requestVote, sourceChannel);
                return;
            }

            Node.SendMessage(sourceChannel, DenyVote);
        }

        public override void ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse, INodeChannel sourceChannel)
        {
            // got vote but we dont care because we are leader already
        }

        public override void ReceiveAppendEntries(AppendEntries appendEntries, INodeChannel sourceChannel)
        {
            if (appendEntries.LeaderTerm > CurrentTerm)
            {
                Node.TranslateToState(RaftNodeState.Follower);
                Node.RaisePacketReceived(appendEntries, sourceChannel);
                return;
            }
        }

        public override void ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse, INodeChannel sourceChannel)
        {
            if (appendEntriesResponse.FollowerTerm > CurrentTerm)
            {
                Node.TranslateToState(RaftNodeState.Follower);
                return;
            }
        }
    }
}
