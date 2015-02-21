using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft
{
    public interface IWorldModel
    {
        void OnElectionStarted();
        void OnAppendEntries();
    }
}
