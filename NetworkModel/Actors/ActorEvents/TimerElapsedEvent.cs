using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors.ActorEvents
{
    public class TimerElapsedEvent: ActorEventBase
    {
        public TimerElapsedEvent(ActorTimer timer)
        {
            Timer = timer;
        }
        public override ActorEventType EventType
        {
            get
            {
                return ActorEventType.TimerElapsed;
            }
        }
        public ActorTimer Timer { get; private set; }
    }
}
