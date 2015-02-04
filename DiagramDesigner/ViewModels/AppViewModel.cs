using Caliburn.Micro;
using DiagramLib.Serialization;
using DiagramLib.ViewModels;
using System.Collections.Generic;

namespace DiagramDesigner
{
    public class AppViewModel : PropertyChangedBase, IShell
    {
        public AppViewModel(IWindowManager wm)
        {
            Diagram1 = new DiagramViewModel(new RaftDiagramDefinition());
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