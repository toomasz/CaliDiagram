using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public abstract class NetworkNode
    {
        public NetworkNode()
        {
            Channels = new List<NodeChannel>();
            InputQueue = new BlockingCollection<object>();
        }
        List<NodeChannel> Channels;
        protected virtual void OnInitialized()
        {

        }
        protected virtual void OnDestroyed()
        {

        }
        protected virtual void OnChannelCreated(NodeChannel channel)
        {

        }
        protected virtual void OnChannelDestroyed(NodeChannel channel)
        {

        }
        protected abstract void OnMessageReceived(NodeChannel channel);
        protected void RequestConnectionTo(string address)
        {

        }
        /// <summary>
        /// Queues message to be sent via channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool SendMessage(NodeChannel channel, object message)
        {
            return false;
        }

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
        protected void BroadcastExcept(object message, NodeChannel except)
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

        void EventLoop()
        {
            Console.Write("Started event queue worker");
            foreach (object evt in InputQueue.GetConsumingEnumerable())
            {
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

                if (evt != null)
                    Console.Write(evt.ToString());
               // MessageReceived(null, evt);


            }

            Console.Write("Worker finished");
        }
    }
}
