using DiagramDesigner.Properties;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagramLib;
using DiagramDesigner.ViewModels;
using System.Windows;
using System.Windows.Threading;
using DiagramLib.Model;


namespace DiagramDesigner.Model
{
    class ModelLoader<TDiagramModel>: ModelLoaderBase<TDiagramModel> where TDiagramModel: new()
    {
        public ModelLoader(DiagramViewModel diagramViewModel):base(diagramViewModel)
        {
            
        }
        protected override NodeBaseViewModel ViewModelFromModel(DiagramNodeBase model)
        {
            if (model is DiagramNodeBig)
                return new DiagramNodeBigViewModel(model.Name) { Location = model.Location };
            if (model is DiagramNodeSmall)
                return new DiagramNodeSmallViewModel(model.Name) { Location = model.Location };
            if (model is DiagramNodeBroker)
                return new DiagramNodeBrokerViewModel(model.Name) { Location = model.Location };
            return null;
        }
        protected override DiagramNodeBase ModelFromViewModel(NodeBaseViewModel viewModel)
        {
            if (viewModel is DiagramNodeSmallViewModel)
                return new DiagramNodeSmall() { Location = viewModel.Location, Name = viewModel.Name };
            if (viewModel is DiagramNodeBigViewModel)
                return new DiagramNodeBig() { Location = viewModel.Location, Name = viewModel.Name };
            if (viewModel is DiagramNodeBrokerViewModel)
                return new DiagramNodeBroker() { Location = viewModel.Location, Name = viewModel.Name };
            return null;
        }

        
    }
}
