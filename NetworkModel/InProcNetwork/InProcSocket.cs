using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    class InProcSocket:INetworkSocket
    {
        public InProcSocket(InProcNetwork network, ChannelType type)
        {
            State = ConnectionState.Closed;
            Type = type;
            this.Network = network;
        }

        InProcNetwork Network;

        public string RemoteAddress
        {
            get;
            internal set;
        }

        public event EventHandler<object> MesageReceived;

        internal void ChangeStateTo(ConnectionState state)
        {
            this.State = state;
            if (StateChanged != null)
                StateChanged(this, state);
        }
        internal void RaiseMessageReceived(object message)
        {
            if (MesageReceived != null)
                MesageReceived(this, message);
            if(ParentServer != null)
            {
                ParentServer.RaiseMessageReceived(this, message);
            }
        }

        public bool SendMessage(object message)
        {
            return SocketSendMessage(this, message);
        }

        internal bool SocketSendMessage(InProcSocket socket, object message)
        {
            if (socket.State != ConnectionState.Established)
                throw new Exception("Only connected sockets can send messages");
            InProcSocket destinationSocket = null;
            if (!Network.EstablishedSockets.TryGetValue(SocketId.RemoteSocketId(socket), out destinationSocket))
                throw new Exception("Failed to send message, destination socket not found");

            Network.TaskScheduler.SchedluleTask(() =>
            {
                destinationSocket.RaiseMessageReceived(message);
            }, TimeSpan.FromMilliseconds(Network.GetConnectionDelay(socket, destinationSocket)));
            return true;
        }
        public ChannelType Type
        {
            get;
            private set;
        }

        public ConnectionState State
        {
            get;
            private set;
        }


        public string LocalAddress
        {
            get;
            internal set;
        }

        /// <summary>
        /// Server channel belongs to, perhaps only for listening socket
        /// </summary>
        internal InProcServer ParentServer
        {
            get;
            set;
        }

        public void Close()
        {
            SocketClosingConnection(this);            
        }

        /// <summary>
        /// Called by socket Close functiion
        /// </summary>
        /// <param name="closingChannel"></param>
        internal void SocketClosingConnection(InProcSocket closingChannel)
        {
            Network.ConnectingSockets.Remove(SocketId.FromSocket(closingChannel));
            if (closingChannel.State == ConnectionState.Closed)
                throw new Exception("Only connected socket can be closed");
            else if (closingChannel.State != ConnectionState.Established)
            {
                closingChannel.ChangeStateTo(ConnectionState.Closed);
            }
            else
            {

                closingChannel.ChangeStateTo(ConnectionState.Closing);
                if (closingChannel.ParentServer != null)
                    closingChannel.ParentServer.RemoveClientChannel(closingChannel);


                Network.EstablishedSockets.Remove(SocketId.FromSocket(closingChannel));

                Network.TaskScheduler.SchedluleTask(() =>
                {
                    // find socket associated with closing one
                    InProcSocket remoteChannel = null;
                    if (!Network.EstablishedSockets.TryGetValue(SocketId.RemoteSocketId(closingChannel), out remoteChannel))
                    {
                        return;
                    }
                    closingChannel.ChangeStateTo(ConnectionState.Closed);
                    remoteChannel.ChangeStateTo(ConnectionState.Closed);

                    // if remote socket belongs to server
                    if (remoteChannel.ParentServer != null)
                        remoteChannel.ParentServer.RemoveClientChannel(remoteChannel);


                    Network.EstablishedSockets.Remove(SocketId.FromSocket(remoteChannel));

                }, TimeSpan.FromMilliseconds(Network.ConnectionCloseLatency));
            }
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", LocalAddress, RemoteAddress);
        }


        public event EventHandler<ConnectionState> StateChanged;


        public void RequestConnectionTo(string remoteAddress)
        {
            RequestClientConnectioTo(this, remoteAddress);
        }

        internal void RequestClientConnectioTo(InProcSocket clientSocket, string destinationAddress)
        {
            if (clientSocket.Type != ChannelType.Client)
                throw new Exception("Server endpoint cannot establish connections");

            // set client socket state to connectting to destination address
            clientSocket.RemoteAddress = destinationAddress;
            clientSocket.ChangeStateTo(ConnectionState.Connecting);

            // add socket to connecting socket dictionary
            Network.ConnectingSockets.Add(SocketId.FromSocket(clientSocket), clientSocket);

            Network.TaskScheduler.SchedluleTask(() =>
            {
                // remove client socket from connecting socket dictionary
                // if that operation fails, that means connect operation was cancelled by client
                if (!Network.ConnectingSockets.Remove(SocketId.FromSocket(clientSocket)))
                    return;

                // Find server by listening channel
                InProcSocket listeningChannel = null;
                if (!Network.ListeningSockets.TryGetValue(clientSocket.RemoteAddress, out listeningChannel))
                {
                    // if listening socket not found, change client state to connection failed
                    clientSocket.ChangeStateTo(ConnectionState.ConnectionFailed);
                    return;
                }

                // Success finding listening socket
                Network.EstablishedSockets.Add(SocketId.FromSocket(clientSocket), clientSocket);

                // Create server client channel
                InProcSocket serverSideChannel = new InProcSocket(Network, ChannelType.Server);
                serverSideChannel.RemoteAddress = clientSocket.LocalAddress;
                serverSideChannel.LocalAddress = listeningChannel.LocalAddress;
                serverSideChannel.ChangeStateTo(ConnectionState.Established);

                Network.EstablishedSockets.Add(SocketId.FromSocket(serverSideChannel), serverSideChannel);

                clientSocket.ChangeStateTo(ConnectionState.Established);

                listeningChannel.ParentServer.AddClientChannel(serverSideChannel);

            }, TimeSpan.FromMilliseconds(Network.ConnectionEstablishLatency));
        }
    }
}
