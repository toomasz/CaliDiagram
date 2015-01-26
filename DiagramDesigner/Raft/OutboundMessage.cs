using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public class OutboundMessage
    {
        public INodeChannel DestinationChannel { get; set; }
        public object Message { get; set; }
    }
}
