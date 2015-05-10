using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModel.Actors.ActorEvents;

namespace NetworkModel.Actors
{
    public partial class ActorBase
    {
        protected virtual void OnInitialized() { }
        protected virtual void OnDestroyed() { }
        protected virtual void OnChannelCreated(ActorChannel channel) { }
        protected virtual void OnChannelRemoved(ActorChannel channel) { }
        protected virtual void OnMessageReceived(ActorChannel channel, object message) { }
        protected virtual void OnCommandReceived(string command) { }


        public event EventHandler<ActorEventBase> ActorEvent;
    }
}
