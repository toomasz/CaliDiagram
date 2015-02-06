
namespace RaftDemo.Raft
{
    public interface INodeChannel
    {
        void SendMessage(object message);
        object Socket { get; }
    }
}
