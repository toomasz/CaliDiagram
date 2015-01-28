using DiagramDesigner.Model;
using DiagramDesigner.Raft;
using DiagramDesigner.ViewModels;
using DiagramLib;
using DiagramLib.ViewModels;
using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagramDesigner
{
    public class SampleDiagramDefinition: DiagramDefinitionBase
    {
        int clientNo = 1;
        int serverNo = 1;
        int brokerNo = 1;


        string GenerateRandomHex(int length)
        {
            var random = new Random();
            return String.Format("{0:x6}", random.Next(0x1000000));
        }
        ICommunication CommunitcationModel = new LocalCommunication();
        public SampleDiagramDefinition()
        {
            //Set custom connector placement strategy
            //ConnectorSideStrategy = new VerticalFavourizedConnectionSrategy();
            AddModelFor<DiagramNodeBrokerViewModel, DiagramNodeBroker>(
                "Broker",
                (p) => new DiagramNodeBrokerViewModel(string.Format("Br{0}", brokerNo++)) { Location = p },
                (vm) => new DiagramNodeBroker() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeBrokerViewModel(m.Name) { Location = m.Location }
            );
            AddModelFor<DiagramNodeSmallViewModel, DiagramNodeSmall>(
                "Client",
                (p) => new DiagramNodeSmallViewModel(string.Format("{0}", GenerateRandomHex(8))) { Location = p },
                (vm) => new DiagramNodeSmall() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeSmallViewModel(m.Name) { Location = m.Location }
            );
            AddModelFor<DiagramNodeBigViewModel, DiagramNodeBig>(
                "Server",
                (p) =>
                    {
                        //this looks nasty
                        return new DiagramNodeBigViewModel(string.Format("S{0}", serverNo++)) 
                        { 
                            Location = p, 
                            NodeSoftware = new RaftNode(CommunitcationModel) 
                        };
                    },
                (vm) => new DiagramNodeBig() { Location = vm.Location, Name = vm.Name },
                (m) =>
                    {
                        return new DiagramNodeBigViewModel(m.Name) 
                        { 
                            Location = m.Location, 
                            NodeSoftware = new RaftNode(CommunitcationModel) 
                        };
                    }
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

        static Brush fill1 = Brushes.Blue;
        static Brush fille2 = Brushes.Red;

        static Brush border1 = Brushes.Green;
        static Brush border2 = Brushes.Black;
    
        public override System.Windows.FrameworkElement CreateVisualForPacket(object packet)
        {
            if (packet is ResponseMessage)
            {
                Ellipse ell = new Ellipse()
                {
                    Width = 10,
                    Height = 10,
                    Fill = fill1,
                    Stroke = fille2,
                    StrokeThickness = 3
                };
            
                return ell;
            }
            else
            {

                Rectangle aRectangle = new Rectangle();
                aRectangle.Width = 10;
                aRectangle.Height = 10;
                aRectangle.Fill = fille2;
                aRectangle.Stroke = border2;
                aRectangle.StrokeThickness = 2;
                
                return aRectangle;
            }
           
        
        }
    }
}
