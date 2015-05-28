using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors
{
    /// <summary>
    /// Server actor listens on given address and also can request connections to other actors
    /// </summary>
    public class ServerActor: ActorBase
    {
        public INetworkServer Server { get; private set; }
        public ServerActor(INetworkModel networkModel, string address):base(networkModel)
        {
            Address = address;
            Server = networkModel.CreateServer(address, false);
            Server.MessageReceived += Server_MessageReceived;
            Server.ClientConnected += Server_ClientConnected;
            Server.ClientDisconnected += Server_ClientDisconnected;
        }
        /// <summary>
        /// Listening address
        /// </summary>
        public string Address { get; private set; }
        public override void Start()
        {
            base.Start();
            Server.StartListening(Address);
        }

        public override void Stop()
        {
            base.Stop();
            Server.Stop();
        }

        Dictionary<INetworkSocket, ActorChannel> ServerSocketToChannel = new Dictionary<INetworkSocket, ActorChannel>();
        
        void Server_ClientDisconnected(object sender, INetworkSocket e)
        {
            
            var channel = ServerSocketToChannel[e];
            
            RaiseChannelRemoved(channel);
        }

        void Server_ClientConnected(object sender, INetworkSocket e)
        {
            ActorChannel actorChannel = new ActorChannel(e);
            ServerSocketToChannel.Add(e, actorChannel);
            RaiseChannelAdded(actorChannel);
        }

        void Server_MessageReceived(object sender, MessageReceivedArgs e)
        {
            var channel = ServerSocketToChannel[e.Socket];
            RaiseMessageReceived(channel, e.Message);
        }
        
    }
}
