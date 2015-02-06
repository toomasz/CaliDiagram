using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels
{
    public class DiagramNodeBrokerViewModel: NodeBaseViewModel
    {
        public DiagramNodeBrokerViewModel(string name)
        {
            this.Name = name;
        }
    }
}
