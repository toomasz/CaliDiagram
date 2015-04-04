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

        Dictionary<string, InProcSocket> listeningSockets = new Dictionary<string, InProcSocket>();
        Dictionary<SocketId, InProcSocket> communicationSockets = new Dictionary<SocketId, InProcSocket>();
        public int ListeningSocketCount
        {
            get { return listeningSockets.Count; }
        }
        public int CommunicationSocketCount
        {
            get { return communicationSockets.Count; }
        }
        int clientNo = 1;
        internal string GetNextClientSocketAddress()
        {
            return string.Format("cl_{0}", clientNo);
            clientNo++;
        }

        public TaskScheduler TaskScheduler { get; private set; }

        internal void RegisterListeningEndpoint(string addres, InProcSocket channel)
        {
            if (channel.Type != ChannelType.Listening)
                throw new Exception("Only listening socket can be registered");
            InProcSocket existingServer = null;
            if (listeningSockets.TryGetValue(addres, out existingServer))
                throw new Exception(string.Format("Address {0} already in use"));

            listeningSockets.Add(addres, channel);
        }
        internal bool UnregisterListeningEnpointFromNetwork(string address)
        {
            return listeningSockets.Remove(address);
        }
        internal void RequestClientConnectioTo(InProcSocket clientChannel, string destinationAddress)
        {
            if(clientChannel.Type != ChannelType.Client)
                throw new Exception("Server endpoint cannot establish connections");

            clientChannel.RemoteAddress = destinationAddress;
            clientChannel.ChangeStateTo(ConnectionState.Connecting);
            communicationSockets.Add(SocketId.FromSocket(clientChannel), clientChannel);
            
            TaskScheduler.SchedluleTask(() => 
            {
                // Find server by listening channel
                InProcSocket listeningChannel = null;
                if(!listeningSockets.TryGetValue(clientChannel.RemoteAddress, out listeningChannel))
                {
                    clientChannel.ChangeStateTo(ConnectionState.ConnectionFailed);
                    return;
                }
                // Create server client channel
                InProcSocket serverSideChannel = new InProcSocket(this, ChannelType.Server);
                serverSideChannel.RemoteAddress = clientChannel.LocalAddress;
                serverSideChannel.LocalAddress = listeningChannel.LocalAddress;
                serverSideChannel.ChangeStateTo(ConnectionState.Established);

                communicationSockets.Add(SocketId.FromSocket(serverSideChannel), serverSideChannel);

                clientChannel.ChangeStateTo(ConnectionState.Established);       
         
                listeningChannel.ParentServer.AddClientChannel(serverSideChannel);

            },TimeSpan.FromMilliseconds(ConnectionEstablishLatency));
        }

        /// <summary>
        /// Called by socket Close functiion
        /// </summary>
        /// <param name="closingChannel"></param>
        internal void SocketClosingConnection(InProcSocket closingChannel)
        {
            if (closingChannel.State != ConnectionState.Established)
                throw new Exception("Only connected socket can be closed");

            closingChannel.ChangeStateTo(ConnectionState.Closed);
            if (closingChannel.ParentServer != null)
                closingChannel.ParentServer.RemoveClientChannel(closingChannel);

            communicationSockets.Remove(SocketId.FromSocket(closingChannel));

            TaskScheduler.SchedluleTask(() =>
            {
                // find socket associated with closing one
                InProcSocket remoteChannel = null;
                if (!communicationSockets.TryGetValue(SocketId.RemoteSocketId(closingChannel), out remoteChannel))
                {                    
                    return;
                }
                
                remoteChannel.ChangeStateTo(ConnectionState.Closed);

                // if remote socket belongs to server
                if (remoteChannel.ParentServer != null)                
                    remoteChannel.ParentServer.RemoveClientChannel(remoteChannel);
                

                communicationSockets.Remove(SocketId.FromSocket(remoteChannel));

            },TimeSpan.FromMilliseconds(ConnectionCloseLatency));
        }
        int GetConnectionDelay(InProcSocket source, InProcSocket destination)
        {
            return ConnectionDefaultLatency;
        }
        internal bool SocketSendMessage(InProcSocket socket, object message)
        {
            if (socket.State != ConnectionState.Established)
                throw new Exception("Only connected sockets can send messages");
            InProcSocket destinationSocket = null;
            if(!communicationSockets.TryGetValue(SocketId.RemoteSocketId(socket), out destinationSocket))
                throw new Exception("Failed to send message, destination socket not found");

            TaskScheduler.SchedluleTask(() =>
            {
                destinationSocket.RaiseMessageReceived(message);
            }, TimeSpan.FromMilliseconds(GetConnectionDelay(socket, destinationSocket)));
            return true;
        }
        

        public INetworkClient CreateClient(string socketAddress = null)
        {
            return new InProcClient(this, socketAddress);
        }

        public INetworkServer CreateServer(string socketAddress)
        {
            var server = new InProcServer(this);
            server.StartListening(socketAddress);
            return server;
        }

        public void Dispose()
        {
            TaskScheduler.Dispose();
        }
    }
}
