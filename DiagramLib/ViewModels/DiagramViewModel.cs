using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;

namespace DiagramLib.ViewModels
{
    public class DiagramViewModel: PropertyChangedBase
    {
        IDiagramDefinition Definition;
        public DiagramViewModel(IDiagramDefinition diagramDefinition)
        {
            Nodes = new ObservableCollection<NodeBaseViewModel>();
            Edges = new ObservableCollection<ConnectionViewModel>();
            AttachDescriptors = new ObservableCollection<NodeBaseViewModel>();
            this.Definition = diagramDefinition;
        }
        public ObservableCollection<NodeBaseViewModel> AttachDescriptors { get; set; } 
        public ObservableCollection<NodeBaseViewModel> Nodes { get; set; }
        public ObservableCollection<ConnectionViewModel> Edges { get; set; }
        public event EventHandler<Point> OnDiagramClick;
        public event EventHandler<NodeBaseViewModel> NodeSelected;
        public event EventHandler<ConnectionViewModel> ConnectionSelected;

        public void ForceRedraw()
        {
            Application.Current.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
        }

        public void DiagramClick(MouseButtonEventArgs args, FrameworkElement el)
        {
           var pos =  args.GetPosition(el);
            if (OnDiagramClick != null)
                OnDiagramClick(this, pos);
        }


        private NodeBaseViewModel _SelectedNode;
        public NodeBaseViewModel SelectedNode
        {
            get { return _SelectedNode; }
            set
            {
                if (_SelectedNode != value)
                {
                    _SelectedNode = value;
                    NotifyOfPropertyChange(() => SelectedNode);
                }
            }
        }
        public void AddNode(NodeBaseViewModel node, Point location)
        {
            node.Location = location;
            Nodes.Add(node);
            Console.WriteLine(string.Format("Added {0}", node.Name));
        }
        public void RemoveNode(NodeBaseViewModel node)
        {
            Nodes.Remove(node);
            var edgesToRemove = Edges.Where(edge => edge.From == node || edge.To == node).ToList();
            foreach (var edge in edgesToRemove)
            {
                AttachDescriptors.Remove(edge.FromDescriptor);
                AttachDescriptors.Remove(edge.ToDescriptor);
                Edges.Remove(edge);
            }
        }

        public void RemoveConnection(ConnectionViewModel edge)
        {
            edge.Dispose();
            if(edge.FromDescriptor != null)
                AttachDescriptors.Remove(edge.FromDescriptor);
            if(edge.ToDescriptor != null)
                AttachDescriptors.Remove(edge.ToDescriptor);
            Edges.Remove(edge);
        }
        public void ClearDiagram()
        {
            foreach (var edge in Edges)
                edge.Dispose();
            Edges.Clear();
            AttachDescriptors.Clear();
            Nodes.Clear();
        }
        public void SelectConnection(ConnectionViewModel edge)
        {
            if (ConnectionSelected != null)
                ConnectionSelected(this, edge);
        }
        public void SelectNode(NodeBaseViewModel vm)
        {
            SelectedNode = vm;
            if (NodeSelected != null)
                NodeSelected(this, vm);
        }

        public void AddConnection(NodeBaseViewModel from, NodeBaseViewModel to)
        {
            var edge = Definition.CreateConnection(from, to);
         
       

            if (edge.FromDescriptor != null)
                AttachDescriptors.Add(edge.FromDescriptor);
            if (edge.ToDescriptor != null)
                AttachDescriptors.Add(edge.ToDescriptor);

            edge.FromDescriptor.Name = "from";
            edge.ToDescriptor.Name = "to";
            Edges.Add(edge);

            var bestDirections = DiagramHelpers.GetAttachDirections(from.Rect, to.Rect);
            edge.AttachPointFrom = from.Attach(bestDirections.Item1, edge, edge.FromDescriptor);
            edge.AttachPointTo = to.Attach(bestDirections.Item2, edge, edge.ToDescriptor);

            edge.UpdateConnection();
        }
    }
}
