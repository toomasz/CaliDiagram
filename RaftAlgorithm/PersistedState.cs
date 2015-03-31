using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftAlgorithm
{
    /// <summary>
    /// Describes the persisted state of a node.
    /// </summary>
    /// <typeparam name="T">Command type for the internal state machine</typeparam>
    public class PersistedState<T>
    {
        private int currentTerm;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedState{T}"/> class.
        /// </summary>
        public PersistedState()
        {
            this.LogEntries = new List<LogEntry<T>>();
        }

        /// <summary>
        /// Gets or sets the last known term.
        /// </summary>
        public int CurrentTerm
        {
            get
            {
                return this.currentTerm;
            }

            set
            {
                if (this.currentTerm == value)
                {
                    return;
                }

                this.currentTerm = value;
                this.VotedFor = null;
            }
        }

        /// <summary>
        /// Gets the last persisted term.
        /// </summary>
        /// <value>
        /// The last persisted term.
        /// </value>
        public int LastPersistedTerm
        {
            get
            {
                return this.LogEntries.Count > 0
                           ? this.LogEntries[this.LogEntries.Count - 1].Term
                           : 0;
            }
        }

        /// <summary>
        /// Gets the last index of the persisted.
        /// </summary>
        /// <value>
        /// The last index of the persisted.
        /// </value>
        public int LastPersistedIndex
        {
            get
            {
                return this.LogEntries.Count > 0 ? this.LogEntries.Count - 1 : -1;
            }
        }

        /// <summary>
        /// Gets or sets the name of the node we voted for (null is none).
        /// </summary>
        public string VotedFor { get; set; }

        /// <summary>
        /// Gets the persisted command.
        /// </summary>
        public IList<LogEntry<T>> LogEntries { get; private set; }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="command">The command.</param>
        public void AddEntry(T command)
        {
            var newEntry = new LogEntry<T>(LogEntries.Count + 1, CurrentTerm, command);
            this.LogEntries.Add(newEntry);
        }

        /// <summary>
        /// Checks if our log is better than the given criteria
        /// </summary>
        /// <param name="lastLogTerm">The last log term.</param>
        /// <param name="lastLogIndex">Last index of the log.</param>
        /// <returns>True if our log contains entries of a greater term or if we have more entries and the same term.</returns>
        /// <remarks>See RAFT specification.</remarks>
        public bool LogIsBetterThan(long lastLogTerm, long lastLogIndex)
        {
            if (this.LogEntries.Count == 0)
            {
                // no log, we are the worst
                return false;
            }

            var lastEntryId = this.LogEntries.Count - 1;
            var lastEntry = this.LogEntries[lastEntryId];
            if (lastEntry.Term > lastLogTerm)
            {
                // if we have more recent info
                return true;
            }

            if (lastEntry.Term < lastLogTerm)
            {
                return false;
            }

            return lastEntryId > lastLogIndex;
        }

        /// <summary>
        /// Check if a given entry has the appropriate characteristics.
        /// </summary>
        /// <param name="index">The requested index.</param>
        /// <param name="term">The expected term.</param>
        /// <returns>true if the entry at index <paramref name="index"/> has the term <paramref name="term"/>.</returns>
        public bool EntryMatches(int index, long term)
        {
            if (index >= this.LogEntries.Count || index < 0)
            {
                return index == -1;
            }

            return this.LogEntries[index].Term == term;
        }

        /// <summary>
        /// Apply a set of <see cref="LogEntry{T}"/> to this log.
        /// </summary>
        /// <param name="prevLogIndex">first index to replace/add</param>
        /// <param name="entries">List of log entries.</param>
        public void AppendEntries(int prevLogIndex, IEnumerable<LogEntry<T>> entries)
        {
            if (entries == null)
            {
                // nothing to add
                return;
            }

            prevLogIndex++;
            foreach (var logEntry in entries)
            {
                if (prevLogIndex == this.LogEntries.Count)
                {
                    this.LogEntries.Add(logEntry);
                }
                else
                {
                    this.LogEntries[prevLogIndex] = logEntry;
                }

                prevLogIndex++;
            }
        }
    }
}
