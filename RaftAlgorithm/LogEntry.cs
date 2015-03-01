using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm
{
    public class LogEntry<T>
    {
        public override string ToString()
        {
            return Data.ToString();
        }
        public T Data
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
