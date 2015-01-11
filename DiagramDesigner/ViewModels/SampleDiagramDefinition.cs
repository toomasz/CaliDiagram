using DiagramLib;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.ViewModels
{
    public class SampleDiagramDefinition: DiagramDefinitionBase
    {
        public SampleDiagramDefinition()
        {
            //Set custom connector placement strategy
            //ConnectorSideStrategy = new VerticalFavourizedConnectionSrategy();
        }
        public override ConnectionViewModel CreateConnection(NodeBaseViewModel from, NodeBaseViewModel to)
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
            
            return connectionViewModel;
        }
    }
}
