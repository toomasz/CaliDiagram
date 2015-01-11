using DiagramLib.Model;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib
{
    class NodeBehaviour
    {
        public string Name
        {
            get;set;
        }
        public string Caption
        {
            get;set;
        }
        public Func<NodeBaseViewModel, DiagramNodeBase> ConvertViewModelToModel;
        public Func<DiagramNodeBase, NodeBaseViewModel> ConvertModelToViewModel;
        public Type Type
        {
            get;
            set;
        }
    }
  
    public abstract class DiagramDefinitionBase
    {
        public Type[] NodeTypes
        {
            get
            {
                return nodeBehaviours.Select(n => n.Value.Type).ToArray();
            }
        }

        Dictionary<string, NodeBehaviour> nodeBehaviours = new Dictionary<string, NodeBehaviour>();
        public void AddModelFor<TViewModel, TModel>(string nodeTypeName, Func<NodeBaseViewModel, DiagramNodeBase> vmToM, Func<DiagramNodeBase, NodeBaseViewModel> modelToVm) 
            where TViewModel: NodeBaseViewModel
            where TModel: DiagramNodeBase
        {
            nodeBehaviours.Add(nodeTypeName, new NodeBehaviour() {
                Type = typeof(TModel), 
                Name = nodeTypeName, 
                Caption = "to do",
                ConvertViewModelToModel = vmToM,
                ConvertModelToViewModel = modelToVm
            });
        }
        public DiagramDefinitionBase()
        {
            ConnectorSideStrategy = new VerticalFavourizedConnectionSrategy();
        }
        public abstract ConnectionViewModel CreateConnection(NodeBaseViewModel from, NodeBaseViewModel to);
        public IConnectorSideStrategy ConnectorSideStrategy {get; set;}
    }
}
