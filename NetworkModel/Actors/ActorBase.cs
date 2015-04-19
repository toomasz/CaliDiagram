using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModel.InProcNetwork;

namespace NetworkModel.Actors
{
    /// <summary>
    /// Base actor class
    /// </summary>
    public class ActorBase
    {
        public INetworkModel NetworkModel { get; private set; }
        public ActorBase(INetworkModel networkModel)
        {
            this.NetworkModel = networkModel;
        }

        public List<NetworkClient> Clients { get; private set; }
        
        public void RequestConnectionTo(string actorAddress)
        {
            NetworkClient Client = new NetworkClient(NetworkModel);
            Clients.Add(Client);
            Client.RemoteAddress = actorAddress;
            Client.IsStarted = true;
           
        }

        public virtual void MessageReceived(object message, ActorChannel source)
        {
            
        }
        public virtual void ChannelCreated(ActorChannel channel)
        {

        }
        public virtual void ChannelDestoryed(ActorChannel channel)
        {
           
        }
    }
}
