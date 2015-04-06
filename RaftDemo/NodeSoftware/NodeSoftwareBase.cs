using Caliburn.Micro;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NetworkModel;

namespace RaftDemo.NodeSoftware
{
    public abstract class NodeSoftwareBase
    {
        public NodeSoftwareBase(INetworkModel networkModel)
        {
            Channels = new List<INetworkSocket>();
            this.NetworkModel = networkModel;
        }
        public INetworkModel NetworkModel
        {
            get;
            private set;
        }       

        public string Id
        {
            get;
            set;
        }

        List<INetworkSocket> Channels
        {
            get;
            set;
        }
        protected virtual void OnInitialized() { }
        protected virtual void OnDestroyed()  {}
        protected virtual void OnChannelCreated(INetworkSocket channel) { }
        protected virtual void OnChannelDestroyed(INetworkSocket channel) { }
        protected virtual void OnMessageReceived(INetworkSocket channel, object message) { } 
        protected virtual void OnCommandReceived(string command){}

        protected virtual void OnTimerElapsed(TimeoutTimer timer) { }

        public event EventHandler<OutboundMessage> OnMessageSent;

        INetworkSocket[] ThreadSafeChannels
        {
            get
            {
                INetworkSocket[] channelsClonned = null;
                lock (Channels)
                {
                    channelsClonned = new INetworkSocket[Channels.Count];
                    Channels.CopyTo(channelsClonned);
                }
                return channelsClonned;
            }
        }
        /// <summary>
        /// Queues message to be sent via channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendMessage(INetworkSocket channel, object message)
        {
            if (!IsStarted)
                return false;
           
            channel.SendMessage(message);
            var onMessageSent = OnMessageSent;
            if (OnMessageSent != null)
                OnMessageSent(this, new OutboundMessage() { Message = message, DestinationChannel = channel });
            return true;
            
            return false;
        }

        /// <summary>
        /// Broadcasts message to all active channels
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void BroadcastMessage(object message)
        {
            foreach (var channel in ThreadSafeChannels)
                SendMessage(channel, message);
        }

        /// <summary>
        /// Broadcasts message to all channels except 'except'
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public void BroadcastExcept(object message, INetworkSocket except)
        {
            foreach (var channel in ThreadSafeChannels)
            {
                if (channel == except)
                    continue;
                SendMessage(channel, message);
            }            
        }
        public event EventHandler<bool> IsStartedChanged;

        private bool _IsStarted;
        public bool IsStarted
        {
            get { return _IsStarted; }
            set
            {
                if (_IsStarted != value)
                {
                    _IsStarted = value;
                    if (IsStartedChanged != null)
                        IsStartedChanged(this, value);
                }
            }
        }
        
        readonly object isStartedLock = new object();

        public void Start()
        {
            if (IsStarted)
                return;
            InputQueue = new BlockingCollection<object>();
            lock (isStartedLock)
            {
                if (IsStarted)
                    return;
                IsStarted = true;
            }            
            OnInitialized();
            StartEventLoop();
        }

        public void Stop()
        {
            if (!IsStarted)
                return;
            InputQueue.CompleteAdding();
            lock (isStartedLock)
            {
                if (IsStarted == false)
                    return;
                IsStarted = false;
            }
           
            InputQueue = null;
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
            t.Name = string.Format("Event loop {0}", Id);
            t.Start();
        }
        void EventLoop()
        {
            Console.WriteLine("Started event queue worker");
            try
            {
                foreach (object evt in InputQueue.GetConsumingEnumerable())
                {
                    if (evt == null)
                        continue;
                    if (!IsStarted)
                        return;

                    /// timers are TimeoutTimer
                    TimeoutTimer timer = evt as TimeoutTimer;
                    if (timer != null)
                        OnTimerElapsed(timer);
                    // strings are commands
                    string command = evt as string;
                    if (command != null)
                        OnCommandReceived(command);

                    // channels as tuples
                    var channelEvent = evt as Tuple<INetworkSocket, bool>;
                    if (channelEvent != null)
                    {
                        if (IsStarted)
                        {
                            if (channelEvent.Item2 == false) // channel removing = false
                                OnChannelCreated(channelEvent.Item1);
                            else
                                OnChannelDestroyed(channelEvent.Item1);
                        }
                    }

                    // MessageReceived(null, evt);
                    InboundMessage message = evt as InboundMessage;
                    if (message != null)
                    {
                        OnMessageReceived(message.SourceChannel, message.Message);
                    }
                }
            }
            catch(OperationCanceledException )
            {
                Console.WriteLine("OperationCanceledException {0}", Id);
            }

            Console.Write("Worker finished");
        }
        /// <summary>
        /// Raise text command to node algorithm
        /// </summary>
        /// <param name="command"></param>
        public void RaiseCommandReceived(string command)
        {
            if (!IsStarted)
                return;
            InputQueue.Add(command);
        }

        /// <summary>
        /// should be called when underlying protocol detects that new connection is established
        /// </summary>
        /// <param name="connection"></param>
        public void RaiseChannelAdded(INetworkSocket channel)
        {
           
            lock(Channels)
                Channels.Add(channel);

            if(IsStarted)
                InputQueue.Add(new Tuple<INetworkSocket, bool>(channel, false));
        }
        /// <summary>
        /// Should be called when underlying protocol detects connection close or failure
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        public void RaiseSocketDead(object socket, object reason = null)
        {
            var channelToRemove = GetChannelBySocket(socket);
            if (channelToRemove == null)
            {
                return;
            }

            if (!Channels.Remove(channelToRemove))
                throw new ArgumentException("Failed to remove channel");

            if (IsStarted)
            {
                if (!InputQueue.IsAddingCompleted)
                    InputQueue.Add(new Tuple<INetworkSocket, bool>(channelToRemove, true));
            }
        }

        public void RaisePacketReceived(object packet, INetworkSocket channel)
        {

            InboundMessage messageObject = new InboundMessage() { Message = packet, SourceChannel = channel };
            if(InputQueue != null)
                if(!InputQueue.IsAddingCompleted)
                      InputQueue.Add(messageObject);
        }
        /// <summary>
        /// Gets channel by underlying socket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public INetworkSocket GetChannelBySocket(object socket)
        {
            return null;
           // lock(Channels)
           //     return Channels.FirstOrDefault(channel => channel.Socket == socket);
        }
    }
}
