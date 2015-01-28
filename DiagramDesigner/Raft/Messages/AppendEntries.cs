using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.Messages
{
    public class AppendEntries : RaftMessageBase
    {
        public override string ToString()
        {
            return "AE";
        }
    }
}
