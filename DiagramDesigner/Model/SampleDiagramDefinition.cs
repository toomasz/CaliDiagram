using DiagramDesigner.Model;
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
            AddModelFor<DiagramNodeBrokerViewModel, DiagramNodeBroker>(
                "node_broker",
                (vm) => new DiagramNodeBroker() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeBrokerViewModel(m.Name) {  Location = m.Location}
            );
            AddModelFor<DiagramNodeSmallViewModel, DiagramNodeSmall>(
                "node_small",
                (vm) => new DiagramNodeSmall() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeSmallViewModel(m.Name) { Location = m.Location }
            );
            AddModelFor<DiagramNodeBigViewModel, DiagramNodeBig>(
                "node_big",
                (vm) => new DiagramNodeBig() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeBigViewModel(m.Name) { Location = m.Location }
            );
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
