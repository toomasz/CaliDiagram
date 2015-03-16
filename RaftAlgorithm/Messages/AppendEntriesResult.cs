
namespace RaftAlgorithm.Messages
{
    public class AppendEntriesResponse : RaftMessageBase
    {
        public AppendEntriesResponse(int followerTerm, string followerId, bool success)
        {
            this.FollowerTerm = followerTerm;
            this.FollowerId = followerId;
            this.Succes = success;
        }
        public override string ToString()
        {
            return "OK";
        }
        /// <summary>
        /// So leader can know who is message from
        /// </summary>
        /// <returns></returns>
        public string FollowerId
        {
            get;
            private set;
        }
        public int FollowerTerm
        {
            get;
            private set;
        }
        /// <summary>
        /// true if follower contained entry matching prevLogIndex and prevLogTerm
        /// </summary>
        /// <returns></returns>
        public bool Succes
        {
            get;
            private set;
        }
    }
}
