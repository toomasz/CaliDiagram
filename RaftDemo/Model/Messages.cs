using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class Message
    {
        public Message(string op)
        {
            this.Operation = op;
        }
        public string Operation
        {
            get;
            private set;
        }
    }
}
