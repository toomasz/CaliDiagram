using CaliDiagram.ViewModels;
using System.Windows;

namespace CaliDiagram.Commands
{
    class AddNodeCommand : DiagramCommand
    {
        NodeTypeBehaviour nodeBehaviour;
        public AddNodeCommand(DiagramViewModel diagram, string description, NodeTypeBehaviour nodeTypeBehaviour):base(diagram)
        {
            this.Description = description;
            this.nodeBehaviour = nodeTypeBehaviour;
            
        }
        public override void OnSelected()
        {
            Diagram.HelpText = Description = string.Format("Add {0} node", nodeBehaviour.Name);
        }
        public override void HandleDiagramClick(Point location)
        {
            Diagram.AddNode(nodeBehaviour.CreateNode(location), location);
        }
    }
}
