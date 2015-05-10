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
            NetworkClientContexts = new List<ClientInfo>();
        }

        /// <summary>
        /// List of client contexts [client - this actor to server - other actor] connections
        /// </summary>
        public List<ClientInfo> NetworkClientContexts { get; private set; } 

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

            foreach (var clientInfo in NetworkClientContexts)
            {
                CreateNetworkClient(clientInfo);
            }
        }

        /// <summary>
        /// Return numbers of client attempting to connect to other actors
        /// </summary>
        public int WorkingClientCount
        {
            get { return NetworkClientContexts.Count(c => c.NetworkClient != null); }
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
        public void AddConnectionTo(string actorAddress)
        {
            ClientInfo client = new ClientInfo {Address = actorAddress};
            NetworkClientContexts.Add(client);
            if (State == ActorState.Started)
                CreateNetworkClient(client);
            
        }

        
        public void Dispose()
        {
            
        }
    }
}
