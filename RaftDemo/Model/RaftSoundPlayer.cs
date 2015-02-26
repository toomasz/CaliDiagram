using RaftAlgorithm;
using RaftDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class RaftSoundPlayer: IRaftEventListener
    {
        SimulationSettings worldSettings;
        public RaftSoundPlayer(SimulationSettings worldSettings)
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
