using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaliDiagram.Commands
{
    class MoveNodeCommand : DiagramCommand
    {
        public MoveNodeCommand(DiagramViewModel diagram):base(diagram)
        {
            Description = "Move";
        }
    }
}
