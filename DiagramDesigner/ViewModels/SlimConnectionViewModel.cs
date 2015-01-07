using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DiagramLib.ViewModels;

namespace DiagramDesigner.ViewModels
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
