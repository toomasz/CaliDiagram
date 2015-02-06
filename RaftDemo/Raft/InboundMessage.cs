
namespace RaftDemo.Raft
{
    /// <summary>
    /// Message origination from another component
    /// </summary>
    public class InboundMessage
    {
        public INodeChannel SourceChannel { get; set; }
        public object Message { get; set; }
    }
}
