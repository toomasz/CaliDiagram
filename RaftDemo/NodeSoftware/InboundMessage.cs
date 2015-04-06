
using NetworkModel;
namespace RaftDemo.NodeSoftware
{
    /// <summary>
    /// Message origination from another component
    /// </summary>
    public class InboundMessage
    {
        public INetworkSocket SourceChannel { get; set; }
        public object Message { get; set; }
    }
}
