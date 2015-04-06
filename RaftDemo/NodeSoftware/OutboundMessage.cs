
using NetworkModel;
namespace RaftDemo.NodeSoftware
{
    public class OutboundMessage
    {
        public INetworkSocket DestinationChannel { get; set; }
        public object Message { get; set; }
    }
}
