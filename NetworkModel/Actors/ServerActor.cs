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
            Server = networkModel.CreateServer(address);
            Server.MessageReceived += Server_MessageReceived;
            Server.ClientConnected += Server_ClientConnected;
            Server.ClientDisconnected += Server_ClientDisconnected;
        }

        void Server_ClientDisconnected(object sender, INetworkSocket e)
        {
            
        }

        void Server_ClientConnected(object sender, INetworkSocket e)
        {
            
        }

        void Server_MessageReceived(object sender, MessageReceivedArgs e)
        {
            base.MessageReceived(e.Message, new ActorChannel(e.Socket));
        }
    }
}
