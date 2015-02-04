using DiagramDesigner.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.State
{
    public abstract class RaftStateBase
    {
        public RaftStateBase(RaftNode Node)
        {
            this.Node = Node;
        }
        protected RaftNode Node
        {
            get;
            private set;
        }
        public abstract void EnterState();
        public abstract void ExitState();
        public virtual void OnTimeout() {  }

        public abstract void ReceiveRequestVote(RequestVote requestVote, INodeChannel sourceChannel);
        public abstract void ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse, INodeChannel sourceChannel);
        public abstract void ReceiveAppendEntries(AppendEntries appendEntries, INodeChannel sourceChannel);
        public abstract void ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse, INodeChannel sourceChannel);


        protected RequestVoteResponse GrantVote
        {
            get
            {
                return new RequestVoteResponse() { VoteGranted = true, CurrentTerm = Node.CurrentTerm };
            }
        }
        protected RequestVoteResponse DenyVote
        {
            get
            {
                return new RequestVoteResponse() { VoteGranted = false, CurrentTerm = Node.CurrentTerm };
            }
        }

        protected int CurrentTerm
        {
            get { return Node.CurrentTerm; }
            set { Node.CurrentTerm = value; }
        }
      //  public virtual void ReceiveMessage(RaftMessageBase message, INodeChannel channel) { }
    }
}
