using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.ViewModels
{
    public class GraphNode1ViewModel : DiagramBaseViewModel
    {
        public GraphNode1ViewModel(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
}
