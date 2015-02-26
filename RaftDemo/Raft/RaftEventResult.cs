using RaftDemo.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft
{
    public class RaftEventResult
    {
        public static RaftEventResult ReplyMessage(RaftMessageBase raftMessage)
        {
            return new RaftEventResult(raftMessage, false);
        }
        public static RaftEventResult BroadcastMessage(RaftMessageBase raftMessage)
        {
            return new RaftEventResult(raftMessage, true);
        }
       
        public static RaftEventResult Empty
        {
            get
            {
                return new RaftEventResult(null, false);
            }
        }
        public RaftEventResult(RaftMessageBase raftMessage, bool doBroadcast)
        {
            this.DoBroadcast = doBroadcast;
            this.MessageToSend = raftMessage;
        }

        /// <summary>
        /// Returns true if Raft requests timer to bet set between TimerFrom and TimerTo
        /// </summary>
        public bool TimerSet
        {
            get;
            private set;
        }

        public int TimerValue
        {
            get;
            private set;
        }
        static readonly Random rnd = new Random();
        public RaftEventResult SetTimer(int timerFrom, int timerTo)
        {
            if (TimerSet)
                throw new InvalidOperationException("Timer already set");
            TimerSet = true;
            TimerValue = rnd.Next(timerFrom, timerTo);
            return this;
        }

        public RaftMessageBase MessageToSend
        {
            get;
            private set;
        }

        public bool DoBroadcast
        {
            get;
            private set;
        }
    }
}
