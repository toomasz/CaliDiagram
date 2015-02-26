using Caliburn.Micro;
using RaftDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels
{
    public class SimulationSettingsViewModel: PropertyChangedBase
    {
        AppViewModel app;
        SimulationSettings simSettings;
        public SimulationSettingsViewModel(AppViewModel app, SimulationSettings simSettings)
        {
            this.app = app;
            this.simSettings = simSettings;
            WorldSpeed = simSettings.WorldSpeedFactor;
        }
        public bool SoundEnabled
        {
            get
            {
                return simSettings.SoundEnabled;
            }
            set
            {
                simSettings.SoundEnabled = value;
            }
        }

        public bool PacketVisualizationEnabled
        {
            get { return simSettings.PacketVisualizationEnabled; }
            set { simSettings.PacketVisualizationEnabled = value; }
        }

        public double WorldSpeed
        {
            get { return simSettings.WorldSpeedFactor; }
            set
            {
                if (simSettings.WorldSpeedFactor != value)
                {
                    simSettings.WorldSpeedFactor = value;
                    NotifyOfPropertyChange(() => WorldSpeed);
                    WorldSpeedStr = string.Format("{0:0.00}x", WorldSpeed);
                }
            }
        }

        private string _WorldSpeedStr;
        public string WorldSpeedStr
        {
            get { return _WorldSpeedStr; }
            set
            {
                if (_WorldSpeedStr != value)
                {
                    _WorldSpeedStr = value;
                    NotifyOfPropertyChange(() => WorldSpeedStr);
                }
            }
        }

  
        public double ServerToServerLatency
        {
            get { return simSettings.ServerToServerLatencySetting; }
            set
            {
                if (simSettings.ServerToServerLatencySetting != value)
                {
                    simSettings.ServerToServerLatencySetting = value;
                    NotifyOfPropertyChange(() => ServerToServerLatency);
                }
            }
        }

        public double ClientToServerLatency
        {
            get { return simSettings.ClientToServerLatencySetting; }
            set
            {
                if (simSettings.ClientToServerLatencySetting != value)
                {
                    simSettings.ClientToServerLatencySetting = value;
                    NotifyOfPropertyChange(() => ClientToServerLatency);
                }
            }
        }
        public string LeaderTimerStr
        {
            get
            {
                return string.Format("{0}ms - {1}ms", LeaderTimerFrom, LeaderTimerTo);
            }
        }
        public string FollowerTimerStr
        {
            get
            {
                return string.Format("{0}ms - {1}ms", FollowerTimerFrom, FollowerTimerTo);
            }
        }
        public int LeaderTimerFrom
        {
            get 
            { 
                return simSettings.LeaderTimeoutFromSetting;
            }
            set
            {
                simSettings.LeaderTimeoutFromSetting = value;
                NotifyOfPropertyChange(() => LeaderTimerStr);
            }
        }

        public int LeaderTimerTo
        {
            get
            {
                return simSettings.LeaderTimeoutToSetting;
            }
            set
            {
                simSettings.LeaderTimeoutToSetting = value;
                NotifyOfPropertyChange(() => LeaderTimerStr);
            }
        }
        public int FollowerTimerFrom
        {
            get
            {
                return simSettings.FollowerTimeoutFromSetting;
            }
            set
            {
                simSettings.FollowerTimeoutFromSetting = value;
                NotifyOfPropertyChange(() => FollowerTimerStr);
            }
        }

        public int FollowerTimerTo
        {
            get
            {
                return simSettings.FollowerTimeoutToSetting;
            }
            set
            {
                simSettings.FollowerTimeoutToSetting = value;
                NotifyOfPropertyChange(() => FollowerTimerStr);
            }
        }
        

        public int ClusterSize
        {
            get { return simSettings.ClusterSize; }
            set
            {
                if (simSettings.ClusterSize != value)
                {
                    simSettings.ClusterSize = value;
                    NotifyOfPropertyChange(() => ClusterSize);
                }
            }
        }
        
        
        
        public void Close()
        {
            app.RightPanel = null;
        }
    }
}
