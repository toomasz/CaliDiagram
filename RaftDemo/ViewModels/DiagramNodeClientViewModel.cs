using CaliDiagram.ViewModels;
using RaftDemo.Model;
using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.ViewModels
{
    public class DiagramNodeClientViewModel : NetworkNodeViewModel
    {
        public DiagramNodeClientViewModel(RaftClient clientSoftware):base(clientSoftware)
        {
            Name = clientSoftware.Id;
        }
    }
}
