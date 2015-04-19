using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModel.InProcNetwork
{
    using TaskScheduling;
    public class InProcNetwork : INetworkModel, IDisposable
    {
        public InProcNetwork()
        {
            ConnectionEstablishLatency = 0;
            TaskScheduler = new TaskScheduler();
        }        

        /// <summary>
        /// How much time it takes to establish connection(dest socket-> conn established)
        /// </summary>
        public int ConnectionEstablishLatency { get; set; }
        /// <summary>
        /// How much time it takes to close connection(dest socket->closed)
        /// </summary>
        public int ConnectionCloseLatency { get; set; }
        /// <summary>
        /// default latency for packets
        /// </summary>
        public int ConnectionDefaultLatency { get; set; }

        /// <summary>
        /// Listening server sockets
        /// </summary>
        internal Dictionary<string, InProcSocket> ListeningSockets = new Dictionary<string, InProcSocket>();

        /// <summary>
        /// SocketIds [local_address, remote_address] -> Socket that have established state
        /// </summary>
        internal Dictionary<SocketId, InProcSocket> EstablishedSockets = new Dictionary<SocketId, InProcSocket>();

        /// <summary>
        /// SocketIds [client_address,server_address] -> Socket that are in 'connecting' state 
        /// </summary>
        internal Dictionary<SocketId, InProcSocket> ConnectingSockets = new Dictionary<SocketId, InProcSocket>();

        public int ListeningSocketCount
        {
            get { return ListeningSockets.Count; }
        }
        public int ConnectedSocketCount
        {
            get { return EstablishedSockets.Count; }
        }
        volatile int clientNo = 1;
        internal string GetNextClientSocketAddress()
        {
            return string.Format("{0}", clientNo++);
        }

        public TaskScheduler TaskScheduler { get; private set; }

        internal void RegisterListeningEndpoint(string addres, InProcSocket channel)
        {
            if (channel.Type != ChannelType.Listening)
                throw new Exception("Only listening socket can be registered");
            InProcSocket existingServer = null;
            if (ListeningSockets.TryGetValue(addres, out existingServer))
                throw new Exception(string.Format("Address {0} already in use"));

            ListeningSockets.Add(addres, channel);
        }
        internal bool UnregisterListeningEnpointFromNetwork(string address)
        {
            return ListeningSockets.Remove(address);
        }
        
        internal int GetConnectionDelay(InProcSocket source, InProcSocket destination)
        {
            return ConnectionDefaultLatency;
        }

        public INetworkSocket CreateClientSocket(string socketAddress = null)
        {
            if (socketAddress == null)
                socketAddress = ":" + GetNextClientSocketAddress();

            return new InProcSocket(this, ChannelType.Client) { LocalAddress = socketAddress };
        }

        public INetworkServer CreateServer(string socketAddress, bool startListening = true)
        {
            var server = new InProcServer(this);
            if(startListening)
                server.StartListening(socketAddress);
            return server;
        }

        public void Dispose()
        {
            TaskScheduler.Dispose();
        }
    }
}
