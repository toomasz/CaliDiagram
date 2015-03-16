namespace RaftAlgorithm
{
    public class LogEntry<T>
    {
        public LogEntry(int commitIndex, int term, T data)
        {
            CommitIndex = commitIndex;
            Term = term;
            Data = data;
        }
        public override string ToString()
        {
            return Data.ToString();
        }
        public T Data
        {
            get;
            set;
        }
        public int CommitIndex
        {
            get;
            set;
        }
        public int Term
        {
            get;
            set;
        }
    }
}
