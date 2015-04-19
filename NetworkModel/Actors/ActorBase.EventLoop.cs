using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkModel.Actors.ActorEvents;

namespace NetworkModel.Actors
{
    public partial class ActorBase
    {
        void RequestStartEventLoop()
        {
            InputQueue = new BlockingCollection<ActorEventBase>();
            Thread t = new Thread(EventLoop);
            t.IsBackground = true;
            t.Name = string.Format("Event loop");
            t.Start();
        }

        void RequestStopEventLoop()
        {
            InputQueue.CompleteAdding();
            InputQueue = null;
        }
        protected void RaiseMessageReceived(ActorChannel channel, object message)
        {
            var messageReceivedEvent = new MessageReceivedEvent(channel, message);
            InputQueue.Add(messageReceivedEvent);
        }
        protected void RaiseChannelAdded(ActorChannel channel)
        {
            var channelAddedEvent = new ChannelAddedEvent(channel);
            InputQueue.Add(channelAddedEvent);
        }
        protected void RaiseChannelRemoved(ActorChannel channel)
        {
            var channelRemovedEvent = new ChannelRemovedEvent(channel);
            InputQueue.Add(channelRemovedEvent);
        }
        /// <summary>
        /// FIFO Queue for actor events like new message, new channel, channel destroyed
        /// </summary>
        public BlockingCollection<ActorEventBase> InputQueue
        {
            get;
            private set;
        }

        void EventLoop()
        {
            
            Console.WriteLine("Started event queue worker");
            try
            {
                State = ActorState.Started;
                foreach (ActorEventBase evt in InputQueue.GetConsumingEnumerable())
                {
                    if (evt == null)
                        continue;
                    // Quit event thread if actor is stopped or in error state
                    if (State == ActorState.Error || State == ActorState.Stopping || State == ActorState.Stopped)
                        return;

                    // channel added
                    var channelAddedEvent = evt as ChannelAddedEvent;
                    if(channelAddedEvent != null)
                    {
                        OnChannelCreated(channelAddedEvent.Channel);
                    }
                    // channel removed
                    var channelRemovedEvent = evt as ChannelRemovedEvent;
                    if (channelAddedEvent != null)
                    {
                        OnChannelRemoved(channelAddedEvent.Channel);
                    }

                    // message received
                    var messageReceivedEvent = evt as MessageReceivedEvent;
                    if(messageReceivedEvent != null)
                    {
                        OnMessageReceived(messageReceivedEvent.Channel, messageReceivedEvent.Message);
                    }
                   
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("OperationCanceledException {0}");
                State = ActorState.Error;
            }

            foreach (var clientInfo in Clients)
            {
                clientInfo.NetworkClient.IsStarted = false;
                clientInfo.NetworkClient = null;
            }
            State = ActorState.Stopped;
            Console.Write("Worker finished");
        }
    }
}
