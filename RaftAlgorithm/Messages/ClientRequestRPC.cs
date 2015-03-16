using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm.Messages
{
    public class ClientRequestRPC<TMessageType> : RaftMessageBase
    {
        public TMessageType Message
        {
            get;
            set;
        }
        public ulong SequenceNumber
        {
            get;
            set;
        }
        
    }
}
