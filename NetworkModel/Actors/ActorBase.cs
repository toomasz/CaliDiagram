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

        public ActorBase(INetworkModel networkModel)
        {
            this.NetworkModel = networkModel;
            NetworkClientContexts = new List<ClientInfo>();
            Channels = new List<ActorChannel>();
        }

        /// <summary>
        /// Network model used by actor
        /// </summary>
        public INetworkModel NetworkModel { get; private set; }

        /// <summary>
        /// List of clientInfo contexts [clientInfo - this actor to server - other actor] connections
        /// </summary>
        public List<ClientInfo> NetworkClientContexts { get; private set; } 

        /// <summary>
        /// Actor state
        /// </summary>
        public ActorState State { get; private set; }

        public List<ActorChannel> Channels { get; private set; } 
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
        /// Return numbers of clientInfo attempting to connect to other actors
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

        void CreateNetworkClient(ClientInfo clientInfo)
        {
            if(clientInfo.NetworkClient != null)
                throw new InvalidOperationException("NetworkClient already created for address " + clientInfo.Address);

            NetworkClient networkClient = new NetworkClient(NetworkModel) {MaxConnectAttempts = -1};
            networkClient.StartConnectingTo(clientInfo.Address);
            networkClient.ConnectionStateChanged += networkClient_ConnectionStateChanged;
            clientInfo.NetworkClient = networkClient;
        }

        void networkClient_ConnectionStateChanged(object sender, bool e)
        {
            throw new NotImplementedException();
        }

    
        public void AddConnectionTo(string actorAddress)
        {
            ClientInfo client = new ClientInfo {Address = actorAddress};
            NetworkClientContexts.Add(client);
            if (State == ActorState.Started)
                CreateNetworkClient(client);
            
        }

        protected void SendMessage(ActorChannel channel, object message)
        {
            channel.Socket.SendMessage(message);
        }

        protected virtual void BroadcastMessage(object message)
        {
           throw  new NotImplementedException();
        }
        
        public void Dispose()
        {
            
        }
    }
}
