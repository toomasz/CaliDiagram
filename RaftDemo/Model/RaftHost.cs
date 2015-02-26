using RaftDemo.Raft;
using RaftDemo.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class RaftHost : NetworkSoftwareBase
    {
        public RaftHost(IRaftEventListener raftEventListener, IRaftNodeSettings raftSettings, string Id)
        {
            this.Id = Id;
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
        protected override void OnMessageReceived(INodeChannel channel, object message)
        {
            RaftMessageBase raftMessage = message as RaftMessageBase;
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
