using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels
{
    public class DiagramNodeClientViewModel : NodeBaseViewModel
    {
        public DiagramNodeClientViewModel(string name)
        {
            this.Name = name;
        }
    }
}
