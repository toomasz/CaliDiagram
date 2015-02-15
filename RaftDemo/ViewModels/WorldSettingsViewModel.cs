using Caliburn.Micro;
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
        public WorldSettingsViewModel(AppViewModel app)
        {
            this.app = app;
        }

        public void Close()
        {
            app.RightPanel = null;
        }
    }
}
