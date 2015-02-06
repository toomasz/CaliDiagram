using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaliDiagram.Commands
{
    class RemoveNodeCommand : DiagramCommand
    {

        public RemoveNodeCommand(DiagramViewModel diagram, string description)
            : base(diagram)
        {
            this.Description = description;
        }
      

        public override void HandleNodeClick(NodeBaseViewModel node)
        {
            Diagram.RemoveNode(node);
        }

        public override void HandleConnectionClick(ConnectionViewModel connection)
        {
            Diagram.RemoveConnection(connection);
        }
    }
}
