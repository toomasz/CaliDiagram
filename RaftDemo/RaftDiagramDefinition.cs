using CaliDiagram;
using CaliDiagram.ViewModels;
using RaftAlgorithm;
using RaftAlgorithm.Messages;
using RaftDemo.Model;
using RaftDemo.NodeSoftware;
using RaftDemo.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RaftDemo
{
    public class RaftDiagramDefinition: DiagramDefinitionBase
    {
        int clientNo = 1;
        int serverNo = 1;
        int brokerNo = 1;

        string GenerateRandomHex(int length)
        {
            var random = new Random();
            return String.Format("{0:x6}", random.Next(0x1000000));
        }

        IRaftEventListener raftEventListener;
        SimulationSettings simulationSettings;

        public RaftDiagramDefinition(RaftSoundPlayer raftEventListener, INetworkModel networkModel, SimulationSettings worldSettings)
        {
            this.raftEventListener = raftEventListener;
            this.simulationSettings = worldSettings;

            // brokers
            AddModelFor<DiagramNodeBrokerViewModel, DiagramNodeBroker>(
                "Broker",
                (p) => new DiagramNodeBrokerViewModel(string.Format("Br{0}", brokerNo++)) { Location = p },
                (vm) => new DiagramNodeBroker() { Location = vm.Location, Name = vm.Name },
                (m) => new DiagramNodeBrokerViewModel(m.Name) { Location = m.Location }
            );

            // clients
            AddModelFor<DiagramNodeClientViewModel, DiagramNodeClient>(
                "Client",
                (p) => 
                {
                    string cliendId = (clientNo++).ToString();
                    RaftClient raftClient = new RaftClient(networkModel, cliendId);
                    return new DiagramNodeClientViewModel(raftClient) { Location = p };
                },
                (vm) => new DiagramNodeClient() { Location = vm.Location, Name = vm.Name },
                (m) =>
                    {
                        RaftClient raftClient = new RaftClient(networkModel, m.Name);
                        return new DiagramNodeClientViewModel(raftClient) { Location = m.Location };
                    }
            );

            // servers
            AddModelFor<DiagramNodeServerViewModel, DiagramNodeServer>(
                "Server",
                (p) =>
                    {
                        serverNo++;
                        string raftNodeId = serverNo.ToString();
                        RaftHost serverSoftware = new RaftHost(networkModel, raftEventListener, simulationSettings, raftNodeId);
                        //this looks nasty
                        return new DiagramNodeServerViewModel(serverSoftware) 
                        { 
                            Location = p
                        };
                    },
                (vm) => new DiagramNodeServer() { Location = vm.Location, Name = vm.Name },
                (m) =>
                    {
                        RaftHost serverSoftware = new RaftHost(networkModel, raftEventListener, simulationSettings, m.Name);
                        return new DiagramNodeServerViewModel(serverSoftware) 
                        { 
                            Location = m.Location                            
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
            if (from is DiagramNodeServerViewModel && to is DiagramNodeServerViewModel)
            {
                connectionViewModel = new ServerToServerConnectionViewModel(from, to, simulationSettings)
                {
                    FromDescriptor = new AttachDescriptorFromViewModel(),
                    ToDescriptor = new AttachDescriptorToViewModel()
                };
            }
            else
            {
                connectionViewModel = new ClientToServerConnectionViewModel(from, to, simulationSettings)
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
    
        public override FrameworkElement CreateVisualForPacket(object packet)
        {
            if (!simulationSettings.PacketVisualizationEnabled)
                return null;
            if(packet is RaftMessageBase)
            {
                TextBlock text = new TextBlock();
                text.Text = packet.ToString();
                text.TextAlignment = TextAlignment.Center;
                text.FontSize = 12;
                text.Height = 15;
                text.Width = 30;
                text.Margin = new Thickness(3, 1, 3, 1);
                text.FontWeight = FontWeights.Bold;
                text.Background = Brushes.Black;
                text.Foreground = Brushes.White;

                return text;
            }     
            else if(packet is Message)
            {
                Message msg = packet as Message;
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Red;
                border.Background = Brushes.White;
                border.CornerRadius = new CornerRadius(1);         
                border.Width = 30;
                border.Height = 15;

                TextBlock text = new TextBlock();
                text.Text = msg.Operation;
                text.Foreground = Brushes.Red;
                text.TextAlignment = TextAlignment.Center;
                text.FontSize = 10;
                border.Child = text;
                return border;
               
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
