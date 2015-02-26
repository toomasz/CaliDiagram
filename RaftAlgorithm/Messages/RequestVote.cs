
namespace RaftAlgorithm.Messages
{
    public class RequestVote : RaftMessageBase
    {
        public string CandidateId { get; set; }
        public int CandidateTerm { get; set; }
        public override string ToString()
        {
            return "RV";
        }
    }
}
