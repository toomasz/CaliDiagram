using DiagramDesigner.Properties;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagramLib;
using DiagramDesigner.ViewModels;

namespace DiagramDesigner.Model
{
    class ModelLoader<TDiagramModel> where TDiagramModel: new()
    {
        DiagramViewModel diagramViewModel;
        XmlSettings<DiagramModel> xmlSettings;
        public ModelLoader(DiagramViewModel diagramViewModel)
        {
            this.diagramViewModel = diagramViewModel;
            xmlSettings = new XmlSettings<DiagramModel>(new Type[]
                {
                    typeof(DiagramNodeBig),
                    typeof(DiagramNodeSmall),
                    typeof(DiagramNodeBroker)
                });
        }
        int n = 1;
        DiagramBaseViewModel ViewModelFromModel(DiagramNodeBase model)
        {
            if (model is DiagramNodeBig)
                return new DiagramNodeBigViewModel() { Location = model.Location, Name = model.Name };
            if (model is DiagramNodeSmall)
                return new DiagramNodeSmallViewModel("c" + (n++).ToString()) { Location = model.Location, Name = model.Name };
            if (model is DiagramNodeBroker)
                return new DiagramNodeBrokerViewModel() { Location = model.Location};
            return null;
        }
        DiagramNodeBase ModelFromViewModel(DiagramBaseViewModel viewModel)
        {
            if (viewModel is DiagramNodeSmallViewModel)
                return new DiagramNodeSmall() { Location = viewModel.Location, Name = ((DiagramNodeSmallViewModel)viewModel).Name };
            if (viewModel is DiagramNodeBigViewModel)
                return new DiagramNodeBig() { Location = viewModel.Location, Name = ((DiagramNodeBigViewModel)viewModel).Name };
            if (viewModel is DiagramNodeBrokerViewModel)
                return new DiagramNodeBroker() { Location = viewModel.Location };
            return null;
        }
        public void SaveDiagram(string filename)
        {
            DiagramModel diagramModel = new DiagramModel();
            Dictionary<DiagramBaseViewModel, DiagramNodeBase> nodeDictionary = new Dictionary<DiagramBaseViewModel, DiagramNodeBase>();
            foreach (var node in diagramViewModel.DiagramItems)
            {
                DiagramNodeBase diagramNode = ModelFromViewModel(node);               
                if (diagramNode != null)
                {
                    diagramModel.Nodes.Add(diagramNode);
                    nodeDictionary.Add(node, diagramNode);
                }
            }

            foreach (var edge in diagramViewModel.Edges)
            {
                diagramModel.Edges.Add(new DiagramConnection()
                {
                    From = nodeDictionary[edge.From],
                    To = nodeDictionary[edge.To]
                });
            }
            xmlSettings.SaveModel(diagramModel, filename);
        }

        public void LoadDiagram(string filename)
        {
            DiagramModel model = xmlSettings.ModelFromSettings(filename);

            diagramViewModel.ClearDiagram();

            Dictionary<DiagramNodeBase, DiagramBaseViewModel> nodeDictionary = new Dictionary<DiagramNodeBase, DiagramBaseViewModel>();
            foreach (var node in model.Nodes)
            {
                DiagramBaseViewModel nodeViewModel = ViewModelFromModel(node);
               
                if (nodeViewModel != null)
                {
                    diagramViewModel.DiagramItems.Add(nodeViewModel);
                    nodeDictionary.Add(node, nodeViewModel);
                }
            }

            foreach (var edge in model.Edges)
            {
                diagramViewModel.AddConnection(nodeDictionary[edge.From], nodeDictionary[edge.To]);                
            }
        }
    }
}
