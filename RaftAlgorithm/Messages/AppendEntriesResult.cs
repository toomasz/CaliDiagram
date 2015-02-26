using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.Messages
{
    public class AppendEntriesResponse : RaftMessageBase
    {
        public AppendEntriesResponse(int followerTerm)
        {
            this.FollowerTerm = followerTerm;
        }
        public override string ToString()
        {
            return "OK";
        }

        public int FollowerTerm
        {
            get;
            private set;
        }
    }
}
