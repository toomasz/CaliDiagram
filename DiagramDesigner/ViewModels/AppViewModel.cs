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
using DiagramLib.Model;

namespace DiagramDesigner
{
    public class AppViewModel : PropertyChangedBase, IShell
    {
        public enum DiagramMode {Move, AddNode1, AddNode2, AddBroker, AddConnection, Delete}
        public AppViewModel(IWindowManager wm)
        {
            Diagram1 = new DiagramViewModel(new SampleDiagramDefinition());
            //Diagram1.ConnectionSelected += Diagram1_ConnectionSelected;
            modelLoader = new DiagramXmlSerializer(Diagram1);            
        }
        DiagramXmlSerializer modelLoader;
        void Diagram1_ConnectionSelected(object sender, ConnectionViewModel e)
        {
          //  if (CurrentMode == DiagramMode.Delete)
         //   {
          //      Diagram1.RemoveConnection(e);
          //  }
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
        


        public DiagramMode CurrentMode { get; set; }

        public IEnumerable<string> ModeList
        {
            get
            {
                return Diagram1.Commands;
            }
        }

        public DiagramViewModel Diagram1 { get; set; }
    
    }

 
}