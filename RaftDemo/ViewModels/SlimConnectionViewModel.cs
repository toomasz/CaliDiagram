using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CaliDiagram.ViewModels;

namespace RaftDemo.ViewModels
{
    public class SlimConnectionViewModel: ConnectionViewModel
    {
        public SlimConnectionViewModel(NodeBaseViewModel from, NodeBaseViewModel to):
            base(from, to)
        {
            StrokeThickness = 3;
        }
    }
}
