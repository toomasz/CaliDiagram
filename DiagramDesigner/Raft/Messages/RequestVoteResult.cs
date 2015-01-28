using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.Messages
{
    public class RequestVoteResult : RaftMessageBase
    {
        public bool VoteGranted { get; set; }
        public override string ToString()
        {
            if (VoteGranted)
                return "Yes";
            else
                return "No";
        }
    }
}
