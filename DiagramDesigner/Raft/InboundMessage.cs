using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    /// <summary>
    /// Message origination from another component
    /// </summary>
    public class InboundMessage
    {
        public INodeChannel SourceChannel { get; set; }
        public object Message { get; set; }
    }
}
