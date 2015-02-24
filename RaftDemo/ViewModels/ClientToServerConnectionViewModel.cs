using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CaliDiagram.ViewModels;
using RaftDemo.Model;

namespace RaftDemo.ViewModels
{
    public class ClientToServerConnectionViewModel: ConnectionViewModel
    {
        public ClientToServerConnectionViewModel(NodeBaseViewModel from, NodeBaseViewModel to, SimulationSettings worldSettings) :
            base(from, to)
        {
            StrokeThickness = 2;
            this.worldSettings = worldSettings;
        }
        SimulationSettings worldSettings;
        public override double Latency
        {
            get
            {
                return worldSettings.ClientToServerLatency;
            }
        }
    }
}
