using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors.ActorEvents
{
    public class MessageReceivedEvent : ActorEventBase
    {
        public MessageReceivedEvent(ActorChannel channel, object message)
        {
            Message = message;
            Channel = channel;
        }
        public ActorChannel Channel { get; private set; }
        public object Message { get; private set; }
    }
}
