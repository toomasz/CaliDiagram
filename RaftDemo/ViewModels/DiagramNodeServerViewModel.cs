using System.Collections.ObjectModel;
using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using RaftDemo.Model;

namespace RaftDemo.ViewModels
{
    class DiagramNodeServerViewModel : NetworkNodeViewModel
    {
        public DiagramNodeServerViewModel(string name, ICommuncatuionModel commModel):base(commModel)
        {
            this.Name = name;
            RaftStateColor = Brushes.DarkBlue;
        }

        private Brush _RaftStateColor;
        public Brush RaftStateColor
        {
            get { return _RaftStateColor; }
            set
            {
                if (_RaftStateColor != value)
                {
                    _RaftStateColor = value;
                    NotifyOfPropertyChange(() => RaftStateColor);
                }
            }
        }
        

        
    }


}