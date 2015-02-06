using CaliDiagram.Model;
using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaliDiagram.Serialization
{
    /// <summary>
    /// Saving and loading diagram as Xml file
    /// </summary>
    public class DiagramXmlSerializer
    {
        
        public DiagramXmlSerializer(DiagramViewModel diagramViewModel)
        {
            this.diagramViewModel = diagramViewModel;
            xmlSettings = new XmlSettings<DiagramModel>(diagramViewModel.Definition.NodeTypes);
           
        }
        DiagramViewModel diagramViewModel;
        XmlSettings<DiagramModel> xmlSettings;

        public void SaveDiagram(string filename)
        {
            DiagramModel diagramModel = new DiagramModel();
            Dictionary<NodeBaseViewModel, DiagramNodeBase> nodeDictionary = new Dictionary<NodeBaseViewModel, DiagramNodeBase>();
            foreach (var node in diagramViewModel.Nodes)
            {
                DiagramNodeBase diagramNode = diagramViewModel.Definition.ViewModelToModel(node);
                if (diagramNode == null)
                    continue;
                diagramModel.Nodes.Add(diagramNode);
                nodeDictionary.Add(node, diagramNode);
                
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

            using (var bm = new DiagramBatchMode(diagramViewModel))
            {
                Dictionary<DiagramNodeBase, NodeBaseViewModel> nodeDictionary = new Dictionary<DiagramNodeBase, NodeBaseViewModel>();
                foreach (var node in model.Nodes)
                {
                    NodeBaseViewModel nodeViewModel =  diagramViewModel.Definition.ModelToViewModel(node);
                    if (nodeViewModel == null)
                        continue;
                   
                    diagramViewModel.AddNode(nodeViewModel, node.Location);

                    nodeDictionary.Add(node, nodeViewModel);
                    
                }
                // Force rendering so we can have sizes of all nodes
              //  diagramViewModel.ForceRedraw();

                foreach (var edge in model.Edges)
                {
                    diagramViewModel.AddConnection(nodeDictionary[edge.From], nodeDictionary[edge.To]);
                }
                // Render again so we can have sizes of attach descriptors
                diagramViewModel.ForceRedraw();

                Console.WriteLine("Model loaded");

                // dont need to call RaiseInitialize, it was allready called in AddNode
                //foreach (var node in diagramViewModel.Nodes)
               //     node.RaiseInitialize();

                foreach (var node in diagramViewModel.AttachDescriptors)
                    node.RaiseInitialize();

                diagramViewModel.ForceRedraw();


            }

            foreach (var conn in diagramViewModel.Edges)
                conn.UpdateConnection();

        }
    }
}
