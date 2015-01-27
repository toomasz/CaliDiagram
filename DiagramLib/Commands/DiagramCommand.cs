using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramLib.Commands
{
    public abstract class DiagramCommand
    {
        protected readonly  DiagramViewModel Diagram;
        public DiagramCommand(DiagramViewModel diagram)
        {
            this.Diagram = diagram;
        }
        public abstract string Description { get; set; }

        public abstract void HandleNodeClick(NodeBaseViewModel node);
        public abstract void HandleConnectionClick(ConnectionViewModel node);
        public abstract void HandleDiagramClick(Point location);
    }
}
