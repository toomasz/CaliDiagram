using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib.Commands
{
    class RemoveNodeCommand : DiagramCommand
    {

        public RemoveNodeCommand(DiagramViewModel diagram, string description)
            : base(diagram)
        {
            this.Description = description;
        }
        public override string Description
        {
            get;
            set;
        }

        public override void HandleNodeClick(NodeBaseViewModel node)
        {
            Diagram.RemoveNode(node);
        }

        public override void HandleDiagramClick(System.Windows.Point location)
        {

        }
        public override void HandleConnectionClick(ConnectionViewModel connection)
        {
            Diagram.RemoveConnection(connection);
        }
    }
}
