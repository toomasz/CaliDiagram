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

        public abstract void ReceiveRequestVote(RequestVote requestVote, INodeChannel channel);
        public abstract void ReceiveRequestVoteResponse(RequestVoteResponse requestVoteResponse);
        public abstract void ReceiveAppendEntries(AppendEntries appendEntries);
        public abstract void ReceiveAppendEntriesResponse(AppendEntriesResponse appendEntriesResponse);

      //  public virtual void ReceiveMessage(RaftMessageBase message, INodeChannel channel) { }
    }
}
