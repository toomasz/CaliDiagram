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
    public partial class ActorBase : IDisposable
    {
        public class ClientInfo
        {
            public string Address;
            public NetworkClient NetworkClient;
        }

        public INetworkModel NetworkModel { get; private set; }
        public ActorBase(INetworkModel networkModel)
        {
            this.NetworkModel = networkModel;
            Clients = new List<ClientInfo>();
        }

        public List<ClientInfo> Clients { get; private set; } 

        /// <summary>
        /// Actor state
        /// </summary>
        public ActorState State { get; private set; }

        /// <summary>
        /// Start actor
        /// </summary>
        public virtual void Start()
        {
            State = ActorState.Starting;
            RequestStartEventLoop();

            foreach (var clientInfo in Clients)
            {
                CreateNetworkClient(clientInfo);
            }
        }

        /// <summary>
        /// Return numbers of client attempting to connect to other actors
        /// </summary>
        public int WorkingClientCount
        {
            get { return Clients.Count(c => c.NetworkClient != null); }
        }


        /// <summary>
        /// Stop actor
        /// </summary>
        public virtual void Stop()
        {
            State = ActorState.Stopping;
            RequestStopEventLoop();
        }

        void CreateNetworkClient(ClientInfo client)
        {
            if(client.NetworkClient != null)
                throw new InvalidOperationException("NetworkClient already created for address " + client.Address);

            NetworkClient networkClient = new NetworkClient(NetworkModel) {MaxConnectAttempts = -1};
            networkClient.StartConnectingTo(client.Address);
            client.NetworkClient = networkClient;
        }
        public void RequestConnectionTo(string actorAddress)
        {
            ClientInfo client = new ClientInfo {Address = actorAddress};
            Clients.Add(client);
            if (State == ActorState.Started)
                CreateNetworkClient(client);
            
        }

        
        public void Dispose()
        {
            
        }
    }
}
