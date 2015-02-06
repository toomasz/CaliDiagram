using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
