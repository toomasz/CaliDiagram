using RaftDemo.Raft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class SimulationSettings: IRaftNodeSettings
    {
        public SimulationSettings()
        {
            SoundEnabled = false;
            WorldSpeedFactor = 0.1;
            ServerToServerLatencySetting = 33;
            ClientToServerLatencySetting = 150;

            LeaderTimeoutFromSetting = 70;
            LeaderTimeoutToSetting = 70;

            FollowerTimeoutFromSetting = 150;
            FollowerTimeoutToSetting = 300;
            
            ClusterSize = 4;
            PacketVisualizationEnabled = true;

        }
        public bool PacketVisualizationEnabled
        {
            get;
            set;
        }
        double ApplySpeedFactor(double value)
        {
            return value * (1.0 / WorldSpeedFactor);
        }
        public bool SoundEnabled
        {
            get;
            set;
        }

        public double ServerToServerLatencySetting
        {
            get;
            set;
        }
        public double ClientToServerLatencySetting
        {
            get;
            set;
        }

        public double ServerToServerLatency
        {
            get
            {
                return ApplySpeedFactor(ServerToServerLatencySetting);
            }
        }
        public double ClientToServerLatency
        {
            get
            {
                return ApplySpeedFactor(ClientToServerLatencySetting);
            }
        }
        public double WorldSpeedFactor
        {
            get;
            set;
        }

        public int ClusterSize
        {
            get;
            set;
        }

        public int LeaderTimeoutFromSetting
        {
            get;
            set;
        }

        public int LeaderTimeoutToSetting
        {
            get;
            set;
        }

        public int FollowerTimeoutFromSetting
        {
            get;
            set;
        }

        public int FollowerTimeoutToSetting
        {
            get;
            set;
        }


        public int LeaderTimeoutFrom
        {
            get { return Convert.ToInt32(ApplySpeedFactor(LeaderTimeoutFromSetting)); }
        }

        public int LeaderTimeoutTo
        {
            get { return Convert.ToInt32(ApplySpeedFactor(LeaderTimeoutToSetting)); }
        }

        public int FollowerTimeoutFrom
        {
            get { return Convert.ToInt32(ApplySpeedFactor(FollowerTimeoutFromSetting)); }
        }

        public int FollowerTimeoutTo
        {
            get { return Convert.ToInt32(ApplySpeedFactor(FollowerTimeoutToSetting)); }
        }
    }
}
