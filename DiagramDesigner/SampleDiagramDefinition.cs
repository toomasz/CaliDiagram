using DiagramDesigner.Model;
using DiagramDesigner.ViewModels;
using DiagramLib;
using DiagramLib.ViewModels;

namespace DiagramDesigner
{
    public class SampleDiagramDefinition: DiagramDefinitionBase
    {
        int clientNo = 1;
        int serverNo = 1;
        int brokerNo = 1;

        public SampleDiagramDefinition()
        {
            //Set custom connector placement strategy
            //ConnectorSideStrategy = new VerticalFavourizedConnectionSrategy();
            AddModelFor<DiagramNodeBrokerViewModel, DiagramNodeBroker>(
                "node_broker",
                (p) => new DiagramNodeBrokerViewModel(string.Format("Br{0}", brokerNo++)) { Location = p },
                (vm) => new DiagramNodeBroker() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeBrokerViewModel(m.Name) { Location = m.Location }
            );
            AddModelFor<DiagramNodeSmallViewModel, DiagramNodeSmall>(
                "node_small",
                (p) => new DiagramNodeSmallViewModel(string.Format("c{0}", clientNo++)) { Location = p },
                (vm) => new DiagramNodeSmall() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeSmallViewModel(m.Name) { Location = m.Location }
            );
            AddModelFor<DiagramNodeBigViewModel, DiagramNodeBig>(
                "node_big",
                (p) => new DiagramNodeBigViewModel(string.Format("S{0}", serverNo++)) { Location = p },
                (vm) => new DiagramNodeBig() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeBigViewModel(m.Name) { Location = m.Location }
            );
        }
        public override ConnectionViewModel CreateConnection(NodeBaseViewModel from, NodeBaseViewModel to)
        {
            ConnectionViewModel connectionViewModel;
            // No connection between same node
            if (from == to)
                return null;
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
