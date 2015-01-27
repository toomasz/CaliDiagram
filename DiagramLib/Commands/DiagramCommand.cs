using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramLib.Commands
{
    public class DiagramCommand
    {
        protected readonly  DiagramViewModel Diagram;
        public DiagramCommand(DiagramViewModel diagram)
        {
            this.Diagram = diagram;
        }
        public virtual string Description { get; set; }

        public virtual void HandleNodeClick(NodeBaseViewModel node) { }
        public virtual void HandleConnectionClick(ConnectionViewModel node) { }
        public virtual void HandleDiagramClick(Point location) { }
        public virtual void OnSelected() { }
    }
}
