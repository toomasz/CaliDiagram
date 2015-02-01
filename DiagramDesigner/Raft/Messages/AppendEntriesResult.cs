using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.Messages
{
    public class AppendEntriesResponse : RaftMessageBase
    {
        public override string ToString()
        {
            return "OK";
        }
    }
}
