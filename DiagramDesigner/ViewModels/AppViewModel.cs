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
        public enum DiagramMode {Move, AddNode1, AddNode2, AddBroker, AddConnection, Delete}
        public AppViewModel(IWindowManager wm)
        {
            Diagram1 = new DiagramViewModel(new SampleDiagramDefinition());
            Diagram1.OnDiagramClick += Diagram1_OnDiagramClick;
            Diagram1.NodeSelected += Diagram1_NodeSelected;
            Diagram1.ConnectionSelected += Diagram1_ConnectionSelected;
            modelLoader = new ModelLoader<DiagramModel>(Diagram1);
            
        }
        ModelLoader<DiagramModel> modelLoader;
        void Diagram1_ConnectionSelected(object sender, ConnectionViewModel e)
        {
            if (CurrentMode == DiagramMode.Delete)
            {
                Diagram1.RemoveConnection(e);
            }
        }
        
        public void SaveDiagram()
        {
            modelLoader.SaveDiagram("Diagram1.xml");
        }
        public void LoadDiagram()
        {
            modelLoader.LoadDiagram("Diagram1.xml");
        }
        
        public void ClearDiagram()
        {
            Diagram1.ClearDiagram();
        }
        private DiagramBaseViewModel prevSelectedNode = null;

        void Diagram1_NodeSelected(object sender, DiagramBaseViewModel e)
        {
            if (CurrentMode == DiagramMode.AddConnection)
            {
                if (prevSelectedNode != null)
                {
                   Diagram1.AddConnection(prevSelectedNode, e);
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
        int clientNo = 1;
        void Diagram1_OnDiagramClick(object sender, System.Windows.Point e)
        {
            if(CurrentMode == DiagramMode.AddNode1)
                Diagram1.DiagramItems.Add(new DiagramNodeSmallViewModel("c" +(clientNo++).ToString()) { Location = e});
            else if (CurrentMode == DiagramMode.AddNode2)
            {
                var newNode = new DiagramNodeBigViewModel() {Location = e};
                Diagram1.DiagramItems.Add(newNode);
            }
            else if (CurrentMode == DiagramMode.AddBroker)
            {
                var newNode = new DiagramNodeBrokerViewModel() { Location = e };
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

        public DiagramViewModel Diagram1 { get; set; }
    
    }

 
}