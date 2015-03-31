using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaftAlgorithm.Messages;

namespace RaftAlgorithm.States
{
    class LogReplicationAgent<T>
    {
        #region fields

        private const int MaxBatch = 20;

        private readonly TimeSpan maxDelay;

        private int minSynchronizedIndex;

        private int lastSentIndex = -1;

        private bool flyingTransaction;

        private DateTime lastSentMessageTime;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReplicationAgent"/> class.
        /// </summary>
        /// <param name="maxDelay">
        /// The max delay between two messages.
        /// </param>
        /// <param name="logSize">
        /// Size of the log.
        /// </param>
        /// <param name="nodeId">Node id, used for log purposes.
        /// </param>
        /// <param name="master">Master logger to capture name.
        /// </param>
        public LogReplicationAgent(TimeSpan maxDelay, int logSize)
        {
            this.maxDelay = maxDelay;
            this.minSynchronizedIndex = logSize - 1;
            this.lastSentMessageTime = DateTime.MinValue;;
        }
        /// <summary>
        /// Gets the index of the min synchronized.
        /// </summary>
        /// <value>
        /// The index of the min synchronized.
        /// </value>
        public int MinSynchronizedIndex
        {
            get
            {
                return this.minSynchronizedIndex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the delay between messages elapsed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if delay elapsed; otherwise, <c>false</c>.
        /// </value>
        private bool DelayElapsed
        {
            get
            {
                return (DateTime.Now - this.lastSentMessageTime) >= this.maxDelay;
            }
        }

        /// <summary>
        /// Gets the next <see cref="AppendEntries{T}"/> message to be sent.
        /// </summary>
        /// <param name="log">Entry log to keep in synchronization.
        /// </param>
        /// <returns>
        /// The message to be send, null if none.
        /// </returns>
        public AppendEntriesRPC<T> GetAppendEntries(IList<LogEntry<T>> log, int leaderTerm, string leaderId)
        {
            // if we are waiting for an answer and delay is not elapsed
            // we do nothing
            if (this.flyingTransaction)
            {
                return null;
            }

            var entriesToSend = Math.Max(
                0, Math.Min(MaxBatch, log.Count - this.minSynchronizedIndex - 1));

            if (entriesToSend == 0 && !this.DelayElapsed)
            {
                return null;
            }

            this.flyingTransaction = true;

            var message = new AppendEntriesRPC<T>(leaderTerm, leaderId)
            {
                PrevLogIndex = this.minSynchronizedIndex,
                PrevLogTerm =
                    this.minSynchronizedIndex < 0
                        ? -1
                        : log[this.minSynchronizedIndex].Term,
                 LogEntries = new List<LogEntry<T>>()
            };
            var offset = this.minSynchronizedIndex + 1;

            for (var i = 0; i < entriesToSend; i++)
            {
                message.LogEntries[i] = log[i + offset];
            }

            this.lastSentIndex = offset + entriesToSend - 1;
            this.lastSentMessageTime = DateTime.Now;
            return message;
        }

        /// <summary>
        /// Processes the append entries acknowledgement message.
        /// </summary>
        /// <param name="success">if set to <c>true</c> [success].</param>
        public void ProcessAppendEntriesAck(bool success)
        {
            this.flyingTransaction = false;
            if (success)
            {
                // if everything was ok
                this.minSynchronizedIndex = this.lastSentIndex;
                return;
            }

            // it fails, log is not synchronised, so we will try on step earlier
            this.minSynchronizedIndex--;
        }
    }
}
