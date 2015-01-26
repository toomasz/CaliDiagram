using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public class NullSoftware : NetworkNode
    {
        public NullSoftware():base(null)
        {

        }
        protected override void OnInitialized()
        {
            
        }
        protected override void OnDestroyed()
        {
            
        }
        protected override void OnChannelCreated(INodeChannel channel)
        {
            
        }
        protected override void OnChannelDestroyed(INodeChannel channel)
        {
            
        }
        protected override void OnMessageReceived(INodeChannel channel, object message)
        {
            
        }
    }
}
