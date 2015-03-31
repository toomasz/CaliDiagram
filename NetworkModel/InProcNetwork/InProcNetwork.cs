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
        }

        TaskScheduler ts = new TaskScheduler();

        public int ConnectionEstablishLatency { get; set; }
        List<InProcServer> Servers = new List<InProcServer>();
        List<InProcClient> Clients = new List<InProcClient>();

        internal void RegisterServerEndpoint(string addres, InProcServer server)
        {
            InProcServer existingServer = null;
            if (serverEndpoints.TryGetValue(addres, out existingServer))
                throw new Exception(string.Format("Address {0} already in use"));

            serverEndpoints.Add(addres, server);
        }

        internal void RequestClientConnectioTo(InProcChannel clientChannel, string destinationAddress)
        {
            if(clientChannel.Type != ChannelType.Client)
                throw new Exception("Server endpoint cannot establish connections");

            clientChannel.RemoteAddress = destinationAddress;
            clientChannel.ChangeStateTo(ConnectionState.Connecting);

            
            ts.SchedluleTask(() => 
            {
                InProcServer destinationServer = null;
                if(!serverEndpoints.TryGetValue(clientChannel.RemoteAddress, out destinationServer))
                {
                    clientChannel.ChangeStateTo(ConnectionState.ConnectionFailed);
                    return;
                }
                // Create client channel
                InProcChannel serverSideChannel = new InProcChannel(this, ChannelType.Server);
                serverSideChannel.RemoteAddress = clientChannel.RemoteAddress;
                serverSideChannel.LocalAddress = destinationServer.ListeningChannel.LocalAddress;                
                clientChannel.ChangeStateTo(ConnectionState.ConnectionEstablished);
                serverSideChannel.ChangeStateTo(ConnectionState.ConnectionEstablished);
                destinationServer.AddClientChannel(serverSideChannel);

            }, TimeSpan.FromMilliseconds(ConnectionEstablishLatency));
        }



        Dictionary<string, InProcServer> serverEndpoints = new Dictionary<string, InProcServer>();
        public INetworkClient CreateClient()
        {
            return new InProcClient(this);
        }

        public INetworkServer CreateServer()
        {
            return new InProcServer(this);
        }

        public void Dispose()
        {
            ts.Dispose();
        }
    }
}
