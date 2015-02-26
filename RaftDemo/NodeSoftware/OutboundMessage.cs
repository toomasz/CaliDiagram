
namespace RaftDemo.NodeSoftware
{
    public class OutboundMessage
    {
        public INodeChannel DestinationChannel { get; set; }
        public object Message { get; set; }
    }
}
