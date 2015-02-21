using RaftDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Raft
{
    public class RaftWorldModel: IWorldModel
    {
        WorldSettings worldSettings;
        public RaftWorldModel(WorldSettings worldSettings)
        {
            this.worldSettings = worldSettings;
        }
        public void OnElectionStarted()
        {
            if (worldSettings.SoundEnabled)
                Console.Beep();
        }

        public void OnAppendEntries()
        {
            if(worldSettings.SoundEnabled)
                Console.Beep(400, 10);
        }
    }
}
