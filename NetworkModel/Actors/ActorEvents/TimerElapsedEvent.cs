using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors.ActorEvents
{
    public class TimerElapsedEvent
    {
        public TimerElapsedEvent(ActorTimer timer)
        {
            Timer = timer;
        }
        public ActorTimer Timer { get; private set; }
    }
}
