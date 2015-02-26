
namespace RaftDemo.Raft
{
    public enum NodeChannelType
    {
        ServerToServer,
        ClientToServer,
        All
    }
    public interface INodeChannel
    {
        void SendMessage(object message);
        object Socket { get; }
        NodeChannelType ChannelType { get; }
    }
}
