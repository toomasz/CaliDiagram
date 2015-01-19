using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public interface INodeChannel
    {
        void SendMessage(object message);
        object Socket { get; }
    }
}
