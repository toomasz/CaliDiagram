using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using DiagramDesigner.Model;
using DiagramDesigner.ViewModels;
using DiagramDesigner.Views;
using DiagramLib;
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
            Diagram1.ConnectionSelected += Diagram1_ConnectionSelected;
        }

        void Diagram1_ConnectionSelected(object sender, ConnectionViewModel e)
        {
            if (CurrentMode == DiagramMode.Delete)
            {
                Diagram1.RemoveConnection(e);
            }
        }

        public void SaveDiagram()
        {
            DiagramModel diagramModel = new DiagramModel();
            Dictionary<DiagramBaseViewModel, DiagramNodeBase> nodeDictionary = new Dictionary<DiagramBaseViewModel, DiagramNodeBase>();
            foreach (var node in Diagram1.DiagramItems)
            {
                DiagramNodeBase diagramNode = null;
                if (node is DiagramNodeSmallViewModel)
                    diagramNode = new DiagramNodeSmall() {Location = node.Location};
                if (node is DiagramNodeBigViewModel)
                    diagramNode = new DiagramNodeBig() {Location = node.Location};
                if (diagramNode != null)
                {
                    diagramModel.Nodes.Add(diagramNode);
                    nodeDictionary.Add(node, diagramNode);
                }
            }

            foreach (var edge in Diagram1.Edges)
            {
                diagramModel.Edges.Add(new DiagramConnection()
                {
                    From = nodeDictionary[edge.From],
                    To = nodeDictionary[edge.To]
                });
            }
            Settings<DiagramModel>.SaveModel(diagramModel, "Diagram1.xml", 
                new Type[] { typeof(DiagramNodeBig), typeof(DiagramNodeSmall)});
        }

        public void LoadDiagram()
        {
            DiagramModel model = Settings<DiagramModel>.ModelFromSettings("Diagram1.xml",new Type[] { typeof(DiagramNodeBig), typeof(DiagramNodeSmall)});

            Diagram1.Edges.Clear();
            Diagram1.DiagramItems.Clear();
            Diagram1.AttachDescriptors.Clear();

            Dictionary<DiagramNodeBase, DiagramBaseViewModel> nodeDictionary = new Dictionary<DiagramNodeBase, DiagramBaseViewModel>();
            foreach (var node in model.Nodes)
            {
                DiagramBaseViewModel nodeViewModel=null;

                if (node is DiagramNodeBig)
                    nodeViewModel = new DiagramNodeBigViewModel() {Location = node.Location};
                if (node is DiagramNodeSmall)
                    nodeViewModel = new DiagramNodeSmallViewModel("a1") { Location = node.Location};
                if (nodeViewModel != null)
                {
                    Diagram1.DiagramItems.Add(nodeViewModel);
                    nodeDictionary.Add(node, nodeViewModel);
                }
            }

            foreach (var edge in model.Edges)
            {
                AddConnection(nodeDictionary[edge.From], nodeDictionary[edge.To]);
            }
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
                Diagram1.DiagramItems.Add(new DiagramNodeSmallViewModel("a1") { Location = e});
            else if (CurrentMode == DiagramMode.AddNode2)
            {
                var newNode = new DiagramNodeBigViewModel() {Location = e};
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

            if (from is DiagramNodeBigViewModel && to is DiagramNodeBigViewModel)
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
            
            Diagram1.AddConnection(connectionViewModel);
        }

        public DiagramViewModel Diagram1 { get; set; }
    
    }

 
}