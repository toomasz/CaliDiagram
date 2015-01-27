using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib.Commands
{
    class MoveNodeCommand : DiagramCommand
    {
        public MoveNodeCommand(DiagramViewModel diagram):base(diagram)
        {
            Description = "Move";
        }
        public override string Description
        {
            get;
            set;
        }

        public override void HandleNodeClick(NodeBaseViewModel node)
        {
            
        }

        public override void HandleConnectionClick(ConnectionViewModel node)
        {
            
        }

        public override void HandleDiagramClick(System.Windows.Point location)
        {
            
        }
    }
}
