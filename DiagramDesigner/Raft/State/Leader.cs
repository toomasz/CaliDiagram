using DiagramDesigner.Raft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft.State
{
    class Leader : RaftStateBase
    {
        public Leader(RaftNode node):base(node)
        {
            
        }
        public override string ToString()
        {
            return "Leader";
        }
        Random rnd = new Random();
        public override void EnterState()
        {
            Node.RaftTimer.SetTimeout(700 + rnd.Next(100) );
        }
        public override void OnTimeout()
        {
            Node.BroadcastMessage(new AppendEntries());
            Node.RaftTimer.SetTimeout(700 + rnd.Next(100));
        }
        public override void ExitState()
        {

        }
    }
}
