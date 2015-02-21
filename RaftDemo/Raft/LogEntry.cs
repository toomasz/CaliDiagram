using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft
{
    public class LogEntry
    {
        public override string ToString()
        {
            return Data;
        }
        public string Data
        {
            get;
            set;
        }
        public int CommitIndex
        {
            get;
            set;
        }
        public int Term
        {
            get;
            set;
        }
    }
}
