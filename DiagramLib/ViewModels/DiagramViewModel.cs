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
using DiagramLib.Commands;
using System.Windows.Controls;
using System.Windows.Media;
using DiagramLib.Views;

namespace DiagramLib.ViewModels
{
    public class DiagramViewModel: PropertyChangedBase, IViewAware
    {
        
        public DiagramViewModel(DiagramDefinitionBase diagramDefinition)
        {
            Nodes = new ObservableCollection<NodeBaseViewModel>();
            Edges = new ObservableCollection<ConnectionViewModel>();
            AttachDescriptors = new ObservableCollection<NodeBaseViewModel>();
            this.Definition = diagramDefinition;
            CanEditNames = false;
            HelpText = "Welcome to diagram designer ! You can drag each node and perfrom operations on it.";

            Commands = new List<DiagramCommand>();
            Commands.Add(new MoveNodeCommand(this));
            Commands.Add(new AddConnectionCommand(this));
            Commands.Add(new RemoveNodeCommand(this, "Remove"));

            foreach (var nodeBehaviour in diagramDefinition.nodeBehaviours)
            {
                Commands.Add(new AddNodeCommand(this, "Add " + nodeBehaviour.Key, nodeBehaviour.Value));
            }
        }

        private string _HelpText;
        public string HelpText
        {
            get { return _HelpText; }
            set
            {
                if (_HelpText != value)
                {
                    _HelpText = value;
                    NotifyOfPropertyChange(() => HelpText);
                }
            }
        }
        

        public DiagramDefinitionBase Definition
        {
            get;
            private set;
        }

        /// <summary>
        /// List of predefined actions for diagram lke 
        /// </summary>
        public List<DiagramCommand> Commands
        {
            get;
            set;
        }

        /// <summary>
        /// Currentyly selected action
        /// </summary>
        private DiagramCommand _SelectedCommand;
        public DiagramCommand SelectedCommand
        {
            get 
            {
                var sc = _SelectedCommand;
                if (sc == null)
                    return new DiagramCommand(this);
                return _SelectedCommand;
            }
            set
            {
                if (_SelectedCommand != value)
                {
                    _SelectedCommand = value;
                    SelectedCommand.OnSelected();
                    NotifyOfPropertyChange(() => SelectedCommand);
                }
            }
        }
        
        public IList<NodeBaseViewModel> AttachDescriptors { get; private set; }
        public IList<NodeBaseViewModel> Nodes { get; private set; }
        public IList<ConnectionViewModel> Edges { get; private set; }
        public event EventHandler<Point> OnDiagramClick;
        public event EventHandler<NodeBaseViewModel> NodeSelected;
        public event EventHandler<ConnectionViewModel> ConnectionSelected;

        public void ForceRedraw()
        {
            Application.Current.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
        }


        void HandleClickCommand(Point pos)
        {
            if (SelectedCommand != null)
                SelectedCommand.HandleDiagramClick(pos);
        }
        
        
        void HandleSelectNodeCommand(NodeBaseViewModel node)
        {
            if(SelectedCommand != null)
                SelectedCommand.HandleNodeClick(node);
        }

        private bool _CanEditLabels;
        public bool CanEditNames
        {
            get { return _CanEditLabels; }
            set
            {
                if (_CanEditLabels != value)
                {
                    _CanEditLabels = value;
                    foreach (var node in Nodes)
                        node.CanEditName = value;
                    foreach (var ad in AttachDescriptors)
                        ad.CanEditName = value;
                    NotifyOfPropertyChange(() => CanEditNames);
                }
            }
        }
        
        public void DiagramClick(MouseButtonEventArgs args, FrameworkElement el)
        {
            
           var pos =  args.GetPosition(el);
           HandleClickCommand(pos);
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

        public bool IsInBatchMode { get; internal set; }

        public void AddNode(NodeBaseViewModel node, Point location)
        {
            if (node == null)
                return;
            if (node.ParentDiagram != null)
                throw new InvalidOperationException("Node is already added to diagram");
            node.ParentDiagram = this;
            node.Location = location;
            Nodes.Add(node);
            ForceRedraw();
            node.RaiseInitialize();
        }
        public void SelectNode(NodeBaseViewModel vm)
        {
            HandleSelectNodeCommand(vm);
            SelectedNode = vm;
            if (NodeSelected != null)
                NodeSelected(this, vm);
        }

        public void RemoveNode(NodeBaseViewModel node)
        {
            node.ParentDiagram = null;
            Nodes.Remove(node);
            node.Dispose();

            var edgesToRemove = Edges.Where(edge => edge.From == node || edge.To == node).ToList();
            foreach (var edge in edgesToRemove)
            {
                RemoveConnection(edge);           
            }
            
        }

        /// <summary>
        /// Creates connection between from and to according to Definition
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Returns null if connection cannot be created</returns>
        public ConnectionViewModel AddConnection(NodeBaseViewModel from, NodeBaseViewModel to)
        {
            var edge = Definition.CreateConnection(from, to);
            if (edge == null)
                return null;
            // prevent showing attach descriptors  when their position is not calculated yet
            // this can be possible not necessary if calculation their location was done before data binding of AttachDescriptors collection
            // but then we would not have size of attach descriptors and size is necessary for calculating position
            if(edge.FromDescriptor != null)
                edge.FromDescriptor.Location = new Point(-1000, -1000);
            if(edge.ToDescriptor != null)
                edge.ToDescriptor.Location = new Point(-1000, -1000);

            edge.ParentDiagram = this;


            if (edge.FromDescriptor != null)
            {
                AttachDescriptors.Add(edge.FromDescriptor);
                
                edge.FromDescriptor.Name = "from";

                if (!IsInBatchMode)
                {
                    ForceRedraw();
                    edge.FromDescriptor.RaiseInitialize();
                }
            }
            if (edge.ToDescriptor != null)
            {
                AttachDescriptors.Add(edge.ToDescriptor);

                edge.ToDescriptor.Name = "to";
                if (!IsInBatchMode)
                {
                    ForceRedraw();
                    edge.ToDescriptor.RaiseInitialize();
                }
            }
            if (!IsInBatchMode)
                ForceRedraw();

            var sides = Definition.ConnectorSideStrategy.GetSidesForConnection(edge);
            edge.AttachPointFrom = from.Attach(sides.FromSide, edge, edge.FromDescriptor);
            edge.AttachPointTo = to.Attach(sides.ToSide, edge, edge.ToDescriptor);

            Edges.Add(edge);

            if (!IsInBatchMode)
                edge.UpdateConnection();
            from.RaiseConnectionAdded(edge);
            to.RaiseConnectionAdded(edge);
            return edge;
        }

        public void SelectConnection(ConnectionViewModel edge)
        {
            SelectedCommand.HandleConnectionClick(edge);
            if (ConnectionSelected != null)
                ConnectionSelected(this, edge);           
        } 

        public void RemoveConnection(ConnectionViewModel edge)
        {
            edge.Dispose();
            if (edge.FromDescriptor != null)
                AttachDescriptors.Remove(edge.FromDescriptor);
            if (edge.ToDescriptor != null)
                AttachDescriptors.Remove(edge.ToDescriptor);
            Edges.Remove(edge);
            edge.From.RaiseConnectionRemoved(edge);
            edge.To.RaiseConnectionRemoved(edge);
        }
        public void ClearDiagram()
        {
            foreach (var edge in Edges)
                edge.Dispose();
            foreach (var node in Nodes)
                node.Dispose();
            foreach (var node in AttachDescriptors)
                node.Dispose();
            Edges.Clear();
            AttachDescriptors.Clear();
            Nodes.Clear();
        }

        /// <summary>
        /// this probably shouldn't be in view model but its still good solution
        /// </summary>
        public Canvas Canvas
        {
            get;
            set;
        }
        public void AttachView(object view, object context = null)
        {
            var dpView = view as DiagramView;
            Canvas = dpView.diagram;
        }
        Canvas parentCanvas(DependencyObject parent)
        {
            Canvas canvas = parent as Canvas;
            if (canvas == null || canvas.Name != "diagram")
            {
                UIElement element = parent as UIElement;
                if (element == null)
                    return null;

                return parentCanvas(VisualTreeHelper.GetParent(parent));
            }
            return canvas;
        }
        public object GetView(object context = null)
        {
            return null;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;
    }

}
