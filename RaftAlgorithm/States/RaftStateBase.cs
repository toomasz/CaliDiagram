using RaftAlgorithm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.States
{
    public abstract class RaftStateBase<T>
    {
        public RaftStateBase(RaftNode<T> Node)
        {
            this.Node = Node;
        }
        protected RaftNode<T> Node
        {
            get;
            private set;
        }
        public abstract RaftEventResult EnterState();
        public virtual RaftEventResult OnTimeout() { return RaftEventResult.Empty; }

        public abstract RaftEventResult ReceiveRequestVote(RequestVote requestVote);
        public abstract RaftEventResult ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse);
        public abstract RaftEventResult ReceiveAppendEntries(AppendEntries appendEntries);
        public abstract RaftEventResult ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse);


        protected RequestVoteResponse GrantVote
        {
            get
            {
                return new RequestVoteResponse() { VoteGranted = true, CurrentTerm = Node.CurrentTerm, VoterId = Node.Id };
            }
        }
        protected RequestVoteResponse DenyVote
        {
            get
            {
                return new RequestVoteResponse() { VoteGranted = false, CurrentTerm = Node.CurrentTerm, VoterId = Node.Id };
            }
        }

        protected int CurrentTerm
        {
            get { return Node.CurrentTerm; }
            set { Node.CurrentTerm = value; }
        }
    }
}
