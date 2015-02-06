using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels
{
    public class DiagramNodeSmallViewModel : NodeBaseViewModel
    {
        public DiagramNodeSmallViewModel(string name)
        {
            this.Name = name;
        }
    }
}
