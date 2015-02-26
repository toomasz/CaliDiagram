using CaliDiagram.ViewModels;

namespace CaliDiagram.Commands
{
    class MoveNodeCommand : DiagramCommand
    {
        public MoveNodeCommand(DiagramViewModel diagram):base(diagram)
        {
            Description = "Move";
        }
       
        public override void OnSelected()
        {
            Diagram.HelpText = "Drag and drop nodes!";
        }
    }
}
