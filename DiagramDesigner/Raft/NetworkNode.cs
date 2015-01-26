using Caliburn.Micro;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public abstract class NetworkNode: PropertyChangedBase
    {
        ICommunication Communication
        {
            get;
            set;
        }
        public NetworkNode(ICommunication communication)
        {
            this.Communication = communication;
            Channels = new List<INodeChannel>();
            InputQueue = new BlockingCollection<object>();
       
        }
        public List<INodeChannel> Channels
        {
            get;
            private set;
        }
        protected abstract void OnInitialized();

        protected virtual void OnDestroyed()
        {
            
        }
        protected virtual void OnChannelCreated(INodeChannel channel)
        {
            
        }
        protected virtual void OnChannelDestroyed(INodeChannel channel)
        {
            
        }
        protected virtual void OnMessageReceived(INodeChannel channel, object message)
        {

        }

        protected virtual void OnCommandReceived(string command)
        {

        }
        protected void RequestConnectionTo(string address)
        {

        }
        /// <summary>
        /// Queues message to be sent via channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool SendMessage(INodeChannel channel, object message)
        {
            channel.SendMessage(message);
            var onMessageSent = OnMessageSent;
            if(OnMessageSent != null)
                OnMessageSent(this, new OutboundMessage() { Message = message, DestinationChannel = channel });
            return false;
        }

        public event EventHandler<OutboundMessage> OnMessageSent;

        /// <summary>
        /// Broadcasts message to all active channels
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void BroadcastMessage(object message)
        {
            foreach (var channel in Channels)
                SendMessage(channel, message);
        }

        /// <summary>
        /// Broadcasts message to all channels except 'except'
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected void BroadcastExcept(object message, INodeChannel except)
        {
            foreach (var channel in Channels)
            { 
                if(channel == except)
                    continue;
                SendMessage(channel, message);
            }
        }


        public void Start()
        {
            OnInitialized();
            StartEventLoop();
        }

        public void Stop()
        {
            InputQueue.CompleteAdding();
            OnDestroyed();
        }

        public BlockingCollection<object> InputQueue
        {
            get;
            private set;
        }

        void StartEventLoop()
        {
            Thread t = new Thread(EventLoop);
            t.IsBackground = true;
            t.Start();
        }


        public void ApplyCommand(string command)
        {
            OnCommandReceived(command);
        }

        void EventLoop()
        {
            Console.WriteLine("Started event queue worker");
            foreach (object evt in InputQueue.GetConsumingEnumerable())
            {
                if (evt == null)
                    continue;
                Console.WriteLine("Received: " + evt.ToString());
                // sttring event - timer elapsed name
                string timerName = evt as string;
                if (timerName != null)
                {
                    if (timerName == "timer1")
                    {
                        // Console.Beep();
                       // RaftTimer.SetElapsed(2000);
                    }
                    continue;
                }

               // MessageReceived(null, evt);
                InboundMessage message = evt as InboundMessage;
                if (message != null)
                {
                    OnMessageReceived(message.SourceChannel, message.Message);
                }
            }

            Console.Write("Worker finished");
        }
    }
}
