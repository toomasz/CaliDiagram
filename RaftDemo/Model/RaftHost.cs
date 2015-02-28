using RaftAlgorithm;
using RaftAlgorithm.Messages;
using RaftAlgorithm.States;
using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Raft = new RaftNode(raftEventListener, raftSettings, Id);            
        }

        public RaftNode Raft
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
            RaftTimer = new TimeoutTimer(this);
            var raftOperationResult = Raft.TranslateToState(RaftNodeState.Follower);
            ProcessRaftResult(raftOperationResult, null);
        }
        protected override void OnDestroyed()
        {
            RaftTimer.Dispose();
        }
        INodeChannel leaderChannel;
        protected override void OnMessageReceived(INodeChannel channel, object message)
        {
            RaftMessageBase raftMessage = message as RaftMessageBase;
            if (raftMessage is AppendEntries)
                leaderChannel = channel;

            if (raftMessage != null)
            {
                var raftOperationResult = Raft.OnMessageReceived(raftMessage);
                ProcessRaftResult(raftOperationResult, channel);
            }

            Message messageFromClient = message as Message;
            if(messageFromClient != null)
            {
                if(Raft.State is Leader)
                {
                    // apply state
                }
                else
                {
                    if(leaderChannel != null)
                        SendMessage(leaderChannel, messageFromClient);
                    // forward message to leader
                }
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

        void ProcessRaftResult(RaftEventResult raftResult, INodeChannel channel)
        {
            if (raftResult == null)
                throw new ArgumentNullException("raftResult");
            if (raftResult.TimerSet)
                RaftTimer.SetRandomTimeout(raftResult.TimerValue, raftResult.TimerValue);
            if (raftResult.MessageToSend != null)
            {
                if (raftResult.DoBroadcast)
                {
                    BroadcastMessage(raftResult.MessageToSend, NodeChannelType.ServerToServer);
                }
                else
                {
                    if (channel == null)
                        throw new InvalidOperationException("Operation started with no channel");
                    SendMessage(channel, raftResult.MessageToSend, NodeChannelType.ServerToServer);
                }
            }
            if (OnRaftEvent != null)
                OnRaftEvent(this, raftResult);
        }

        
    }
}
