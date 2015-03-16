
namespace RaftAlgorithm
{
    public interface IRaftEventListener
    {
        void OnElectionStarted();
        void OnAppendEntries();
    }
}
