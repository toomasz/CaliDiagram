using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.ViewModels
{
    public class DiagramNodeSmallViewModel : NodeBaseViewModel
    {
        public DiagramNodeSmallViewModel(string name)
        {
            this.Name = name;
        }
    }
}
