using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft
{
    public interface IRaftNodeSettings
    {
        int ClusterSize { get; }
        int LeaderTimeoutFrom { get; }
        int LeaderTimeoutTo { get; }

        int FollowerTimeoutFrom { get; }
        int FollowerTimeoutTo { get; }
    }
}
