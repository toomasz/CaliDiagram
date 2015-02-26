
namespace RaftDemo.Raft.Messages
{
    public class Message
    {
        public int Clock { get; set; }
        public string State { get; set; }
        public override string ToString()
        {
            return string.Format("MSG: {0} {1}", Clock, State);
        }
    }
    public class ResponseMessage
    {

    }
   
}
