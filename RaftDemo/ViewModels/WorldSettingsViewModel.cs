using Caliburn.Micro;
using RaftDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels
{
    public class WorldSettingsViewModel: PropertyChangedBase
    {
        AppViewModel app;
        WorldSettings worldSettings;
        public WorldSettingsViewModel(AppViewModel app, WorldSettings worldSettings)
        {
            this.app = app;
            this.worldSettings = worldSettings;
        }
        public bool SoundEnabled
        {
            get
            {
                return worldSettings.SoundEnabled;
            }
            set
            {
                worldSettings.SoundEnabled = value;
            }
        }
        public void Close()
        {
            app.RightPanel = null;
        }
    }
}
