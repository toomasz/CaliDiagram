using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels.Actors
{
    public class BrokerViewModel: NodeBaseViewModel
    {
        public BrokerViewModel(string name)
        {
            this.Name = name;
        }
    }
}
