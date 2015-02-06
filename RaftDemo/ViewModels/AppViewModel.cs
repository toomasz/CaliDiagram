using Caliburn.Micro;
using CaliDiagram.Serialization;
using CaliDiagram.ViewModels;
using System.Collections.Generic;

namespace RaftDemo
{
    public class AppViewModel : PropertyChangedBase, IShell
    {
        public AppViewModel(IWindowManager wm)
        {
            Diagram1 = new DiagramViewModel(new RaftDiagramDefinition());
            modelLoader = new DiagramXmlSerializer(Diagram1);            
        }
        DiagramXmlSerializer modelLoader;

        public void About()
        {

        }
       
        
        public void SaveDiagram()
        {
            modelLoader.SaveDiagram("RaftModel.xml");
        }
        public void LoadDiagram()
        {
            modelLoader.LoadDiagram("RaftModel.xml");
        }
        
        public void ClearDiagram()
        {
            Diagram1.ClearDiagram();
        }

        public DiagramViewModel Diagram1 { get; set; }

        private bool _CanEditNames;
        public bool CanEditNames
        {
            get { return _CanEditNames; }
            set
            {
                if (_CanEditNames != value)
                {
                    _CanEditNames = value;
                    Diagram1.CanEditNames = value;
                    NotifyOfPropertyChange(() => CanEditNames);
                }
            }
        }



        public void Close()
        {
            Diagram1.Dispose();
        }
    }

 
}