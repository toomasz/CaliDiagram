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
                Diagram1.RemoveNode(e);
            }
        }

        void Diagram1_OnDiagramClick(object sender, System.Windows.Point e)
        {
            if(CurrentMode == DiagramMode.AddNode1)
                Diagram1.DiagramItems.Add(new GraphNode1ViewModel("a1") { Location = e});
            else if (CurrentMode == DiagramMode.AddNode2)
            {
                var newNode = new GraphNodeViewModel() {Location = e};
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
            ConnectionViewModel connectionViewModel;

            if (from is GraphNodeViewModel && to is GraphNodeViewModel)
            {
                connectionViewModel = new ThickConnectionViewModel(from, to)
                {
                    FromDescriptor = new AttachDescriptorFromViewModel(),
                    ToDescriptor = new AttachDescriptorToViewModel()
                };
            }
            else
            {
                connectionViewModel = new SlimConnectionViewModel(from, to)
                {
                    FromDescriptor = new AttachDescriptorFromViewModel(),
                    ToDescriptor = new AttachDescriptorToViewModel()
                };
            }

            if (connectionViewModel.FromDescriptor != null)
                Diagram1.AttachDescriptors.Add(connectionViewModel.FromDescriptor);
            if (connectionViewModel.ToDescriptor != null)
                Diagram1.AttachDescriptors.Add(connectionViewModel.ToDescriptor);

            Diagram1.Edges.Add(connectionViewModel);
            connectionViewModel.UpdateConnection();
        }

        public DiagramViewModel Diagram1 { get; set; }
    
    }

 
}