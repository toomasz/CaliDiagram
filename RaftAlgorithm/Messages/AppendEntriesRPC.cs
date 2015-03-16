
using System.Collections.Generic;

namespace RaftAlgorithm.Messages
{
    public class AppendEntriesRPC<T> : RaftMessageBase
    {
        public AppendEntriesRPC(int leaderTerm, string leaderId)
        {
            this.LeaderId = leaderId;
            this.LeaderTerm = leaderTerm;
        }
        public override string ToString()
        {
            return "AE";
        }

        /// <summary>
        /// Leader term number
        /// </summary>
        /// <returns></returns>
        public int LeaderTerm
        {
            get;
            set;
        }

        /// <summary>
        /// Id of leader
        /// </summary>
        /// <returns></returns>
        public string LeaderId
        {
            get;
            set;
        }
        /// <summary>
        /// Index of log entry immedietaly proceeding new ones
        /// </summary>
        /// <returns></returns>
        public int PrevLogIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Index of prevLogIndex log entry
        /// </summary>
        /// <returns></returns>
        public int PrevLogTerm
        {
            get;
            set;
        }
        /// <summary>
        /// Log entries to store(may be null)
        /// </summary>
        /// <returns></returns>
        public List<LogEntry<T>> LogEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Leader commit index
        /// </summary>
        public int LeaderCommit
        {
            get;
            set;
        }
    }
}
