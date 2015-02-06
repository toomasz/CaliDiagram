
namespace RaftDemo.Raft
{
    public class OutboundMessage
    {
        public INodeChannel DestinationChannel { get; set; }
        public object Message { get; set; }
    }
}
