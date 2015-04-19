using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors.ActorEvents
{
    public class ChannelAddedEvent : ActorEventBase
    {
        public ChannelAddedEvent(ActorChannel channel)
        {
            Channel = channel;
        }
        public ActorChannel Channel { get; private set; }
    }
}
