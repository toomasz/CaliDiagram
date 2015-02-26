using System;

namespace CaliDiagram.ViewModels
{
    public class DiagramBatchMode: IDisposable
    {
        private DiagramViewModel diagram;
        public DiagramBatchMode(DiagramViewModel diagram)
        {
            this.diagram = diagram;
            diagram.IsInBatchMode = true;
        }

        public void Dispose()
        {
            diagram.IsInBatchMode = false;
        }
    }
}
