using CaliDiagram.ViewModels;

namespace CaliDiagram.Commands
{
    class RemoveNodeCommand : DiagramCommand
    {
        public RemoveNodeCommand(DiagramViewModel diagram, string description)
            : base(diagram)
        {
            this.Description = description;
        }

        public override void OnSelected()
        {
            Diagram.HelpText = "Remove any node by clicking it.";
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
