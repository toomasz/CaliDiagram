using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using DiagramDesigner.ViewModels;
using DiagramLib.ViewModels;
using DiagramLib.Views;

namespace DiagramDesigner
{
    public class AppViewModel : PropertyChangedBase, IShell
    {
        public enum DiagramMode {Move, AddNode1, AddNode2, AddConnection, Delete}
        public AppViewModel(IWindowManager wm)
        {
            Diagram1 = new DiagramViewModel();
            Diagram1.OnDiagramClick += Diagram1_OnDiagramClick;
            Diagram1.NodeSelected += Diagram1_NodeSelected;
        }

        private DiagramBaseViewModel prevSelectedNode = null;

        void Diagram1_NodeSelected(object sender, DiagramBaseViewModel e)
        {
            if (CurrentMode == DiagramMode.AddConnection)
            {
                if (prevSelectedNode != null)
                {
                    AddConnection(prevSelectedNode, e);
                    prevSelectedNode = null;
                }
                else
                {
                    prevSelectedNode = e;
                }
                
            }
            if (CurrentMode == DiagramMode.Delete)
            {
                Diagram1.DiagramItems.Remove(e);
                var edgesToRemove = Diagram1.Edges.Where(edge => edge.From == e || edge.To == e).ToList();
                foreach (var edge in edgesToRemove)
                {
                    Diagram1.AttachDescriptors.Remove(edge.FromDescriptor);
                    Diagram1.AttachDescriptors.Remove(edge.ToDescriptor);
                    Diagram1.Edges.Remove(edge);
                }


            }
        }

        void Diagram1_OnDiagramClick(object sender, System.Windows.Point e)
        {
            if(CurrentMode == DiagramMode.AddNode1)
                Diagram1.DiagramItems.Add(new GraphNode1ViewModel("a1") { X = e.X, Y=e.Y});
            else if (CurrentMode == DiagramMode.AddNode2)
            {
                var newNode = new GraphNodeViewModel() {X = e.X, Y = e.Y};
                Diagram1.DiagramItems.Add(newNode);
            }
        }

        public DiagramMode CurrentMode { get; set; }

        public IEnumerable<DiagramMode> ModeList
        {
            get
            {
                return Enum.GetValues(typeof(DiagramMode)).Cast<DiagramMode>();
            }
        }
        void AddConnection(DiagramBaseViewModel from, DiagramBaseViewModel to)
        {
            var fromAttachDescriptor = new AttachDescriptorFromViewModel();
            var toAttachDescriptor = new AttachDescriptorToViewModel();
            
            var connVm = new ConnectionViewModel(from, to, fromAttachDescriptor, toAttachDescriptor, Diagram1);
            Diagram1.AttachDescriptors.Add(fromAttachDescriptor);
            Diagram1.AttachDescriptors.Add(toAttachDescriptor);
            Diagram1.Edges.Add(connVm);
        }

        public DiagramViewModel Diagram1 { get; set; }
    
    }

 
}