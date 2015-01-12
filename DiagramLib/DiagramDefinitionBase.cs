using DiagramLib.Model;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib
{
    internal class NodeBehaviour
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
        public Type TypeViewModel
        {
            get;
            set;
        }
        public Type TypeModel
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
                return nodeBehaviours.Select(n => n.Value.TypeModel).ToArray();
            }
        }

        Dictionary<string, NodeBehaviour> nodeBehaviours = new Dictionary<string, NodeBehaviour>();
        public void AddModelFor<TViewModel, TModel>(string nodeTypeName, Func<NodeBaseViewModel, DiagramNodeBase> vmToM, Func<DiagramNodeBase, NodeBaseViewModel> modelToVm) 
            where TViewModel: NodeBaseViewModel
            where TModel: DiagramNodeBase
        {
            nodeBehaviours.Add(nodeTypeName, new NodeBehaviour() {
                TypeViewModel = typeof(TViewModel), 
                TypeModel = typeof(TModel),
                Name = nodeTypeName, 
                Caption = "to do",
                ConvertViewModelToModel = vmToM,
                ConvertModelToViewModel = modelToVm
            });
        }

        internal NodeBaseViewModel ModelToViewModel(DiagramNodeBase model)
        {
            var ctx = nodeBehaviours.FirstOrDefault(b => b.Value.TypeModel == model.GetType());
            return ctx.Value.ConvertModelToViewModel(model);
        }
        internal DiagramNodeBase ViewModelToModel(NodeBaseViewModel viewModel)
        {
            var ctx = nodeBehaviours.FirstOrDefault(b => b.Value.TypeViewModel == viewModel.GetType());
            return ctx.Value.ConvertViewModelToModel(viewModel);
        }
        public DiagramDefinitionBase()
        {
            ConnectorSideStrategy = new VerticalFavourizedConnectionSrategy();
        }
        public abstract ConnectionViewModel CreateConnection(NodeBaseViewModel from, NodeBaseViewModel to);
        public IConnectorSideStrategy ConnectorSideStrategy {get; set;}
    }
}
