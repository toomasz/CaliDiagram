using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft.Messages
{
    public class RequestVote : RaftMessageBase
    {
        public string CandidateId { get; set; }
        public int CandidateTerm { get; set; }
        public override string ToString()
        {
            return "RV";
        }
    }
}
