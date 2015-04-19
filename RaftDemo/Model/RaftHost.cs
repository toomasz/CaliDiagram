using RaftAlgorithm;
using RaftAlgorithm.Messages;
using RaftAlgorithm.States;
using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModel;

namespace RaftDemo.Model
{
    public class RaftHost : NodeSoftwareBase
    {
        public RaftHost(INetworkModel networkModel, IRaftEventListener raftEventListener, IRaftNodeSettings raftSettings, string Id):
            base(networkModel)
        {
            this.Id = Id;
            if (Id == null)
                throw new ArgumentException("Id");
            Raft = new RaftNode<string>(raftEventListener, raftSettings, Id);
            Server = networkModel.CreateServer(Id, startListening: false);
        }
        INetworkServer Server { get; set; }
        public RaftNode<string> Raft
        {
            get;
            private set;
        }

        TimeoutTimer RaftTimer
        {
            get;
            set;
        }       

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Server.StartListening(Id);
            RaftTimer = new TimeoutTimer(this);
            var raftOperationResult = Raft.Start();
            ProcessRaftResult(raftOperationResult);
        }
        protected override void OnDestroyed()
        {
            RaftTimer.Dispose();
            Server.Dispose();
        }
        INetworkSocket leaderChannel;
        protected override void OnMessageReceived(INetworkSocket channel, object message)
        {
            RaftMessageBase raftMessage = message as RaftMessageBase;
            if (raftMessage is AppendEntriesRPC<string>)
                leaderChannel = channel;

            Message messageFromClient = message as Message;
            if(messageFromClient != null)
            {
                if(Raft.State == RaftNodeState.Leader)
                {
                    raftMessage = new ClientRequestRPC<string>()
                    {
                        Message = messageFromClient.Operation,
                        SequenceNumber = 0
                    };
                }
                else
                {
                    if(leaderChannel != null)
                        SendMessage(leaderChannel, messageFromClient);
                    // forward message to leader
                }
            }

            if (raftMessage != null)
            {
                var raftOperationResult = Raft.OnMessageReceived(raftMessage);
                ProcessRaftResult(raftOperationResult, channel);
            }

            
        }
        protected override void OnTimerElapsed(TimeoutTimer timer)
        {
            if (timer == RaftTimer)
            {
                var raftOperationResult = Raft.OnTimerElapsed();
                ProcessRaftResult(raftOperationResult, null);
            }
        }
        public event EventHandler<RaftEventResult> OnRaftEvent;

        void ProcessRaftResult(RaftEventResult raftResult, INetworkSocket channel = null)
        {
            if (raftResult == null)
                throw new ArgumentNullException("raftResult");
            if (raftResult.TimerSet)
                RaftTimer.SetRandomTimeout(raftResult.TimerValue, raftResult.TimerValue);
            if (raftResult.MessageToSend != null)
            {
                if (raftResult.DoBroadcast)
                {
                    BroadcastMessage(raftResult.MessageToSend);
                }
                else
                {
                    if (channel == null)
                        throw new InvalidOperationException("Operation started with no channel");
                    SendMessage(channel, raftResult.MessageToSend);
                }
            }
            if (OnRaftEvent != null)
                OnRaftEvent(this, raftResult);
        }
        
    }
}
