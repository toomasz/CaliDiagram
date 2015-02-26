using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft.Messages
{
    public class AppendEntries : RaftMessageBase
    {
        public AppendEntries(int leaderTerm, string leaderId)
        {
            this.LeaderId = leaderId;
            this.LeaderTerm = leaderTerm;
        }
        public override string ToString()
        {
            return "AE";
        }
        public int LeaderTerm
        {
            get;
            set;
        }
        public string LeaderId
        {
            get;
            set;
        }
    }
}
