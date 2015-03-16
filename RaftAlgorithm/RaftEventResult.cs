using RaftAlgorithm.Messages;
using System;

namespace RaftAlgorithm
{
    /// <summary>
    /// Result of eny operation performed on raft algorithm like receiving message, elapsing raft timer
    /// </summary>
    public class RaftEventResult
    {
        /// <summary>
        /// Create RaftEventResult object
        /// </summary>
        /// <param name="raftMessage">Message to broadcast or reply</param>
        /// <param name="doBroadcast">True of message need broadcasting instead of replying</param>
        public RaftEventResult(RaftMessageBase raftMessage, bool doBroadcast)
        {
            this.DoBroadcast = doBroadcast;
            this.MessageToSend = raftMessage;
        }
        /// <summary>
        /// Create event that will result in replying to port that request came from
        /// </summary>
        /// <param name="raftMessage"></param>
        /// <returns></returns>
        public static RaftEventResult ReplyMessage(RaftMessageBase raftMessage)
        {
            return new RaftEventResult(raftMessage, false);
        }
        /// <summary>
        /// Create event that will result in broadcasting message
        /// </summary>
        /// <param name="raftMessage"></param>
        /// <returns></returns>
        public static RaftEventResult BroadcastMessage(RaftMessageBase raftMessage)
        {
            return new RaftEventResult(raftMessage, true);
        }
        
        /// <summary>
        /// Create empty Raft event result
        /// </summary>
        public static RaftEventResult Empty
        {
            get
            {
                return new RaftEventResult(null, false);
            }
        }
        /// <summary>
        /// Returns true if Raft  algorithm requests timer to bet set
        /// </summary>
        public bool TimerSet
        {
            get;
            private set;
        }
        /// <summary>
        /// Value of timer
        /// </summary>
        public int TimerValue
        {
            get;
            private set;
        }
        static readonly Random rnd = new Random();

        /// <summary>
        /// Request to set raft timer to random value between timerFrom and timerTo
        /// </summary>
        /// <param name="timerFrom"></param>
        /// <param name="timerTo"></param>
        /// <returns></returns>
        public RaftEventResult SetTimer(int timerFrom, int timerTo)
        {
            if (TimerSet)
                throw new InvalidOperationException("Timer already set");
            TimerSet = true;
            TimerValue = rnd.Next(timerFrom, timerTo);
            return this;
        }

        /// <summary>
        /// Massage to reply or broadcast
        /// </summary>
        public RaftMessageBase MessageToSend
        {
            get;
            private set;
        }
        /// <summary>
        /// True of MessageToSend needs to be broadcasted
        /// </summary>
        public bool DoBroadcast
        {
            get;
            private set;
        }
    }
}
