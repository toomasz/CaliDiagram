using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkModel;
using NetworkModel.InProcNetwork;
using NFluent;

namespace NetworkTest
{
    [TestClass]
    public class BasicNetworkTest
    {
        INetworkModel CreateNetModel()
        {
            return new InProcNetwork();
        }
        [TestMethod]
        public void NodeCreation()
        {
            INetworkModel network1 = CreateNetModel();
            NetworkNode node1 = new NetworkNode("node1", network1);
            NetworkNode node2 = new NetworkNode("node2", network1);
            Check.That(() => new NetworkNode("node1", network1)).ThrowsAny();
            Check.That(node1.Name).IsEqualTo("node1");
            Check.That(node2.Name).IsEqualTo("node2");
            
        }

        [TestMethod]
        public void ClientServerConnectionTest()
        {
            InProcNetwork network1 = new InProcNetwork()
            {
                ConnectionEstablishLatency = 30
            };
            
            INetworkServer server = network1.CreateServer();
            INetworkClient client = network1.CreateClient();

            Check.That(server.ListeningChannel).IsNull();

            Check.That(server.StartListening("server")).IsTrue();
            Check.That(server.ListeningChannel.LocalAddress).IsEqualTo("server");
            

            Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Closed);
            
            client.RequestConnectionTo("server");
            Check.That(client.ClientChannel.RemoteAddress).IsEqualTo("server");
            Check.That(client.ClientChannel.Type).IsEqualTo(ChannelType.Client);
            Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Connecting);
            Thread.Sleep(30);
            Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.ConnectionEstablished);
            Check.That(server.ClientChannels.Count).IsEqualTo(1);

            var serverSideChannel = server.ClientChannels[0];
            Check.That(serverSideChannel.LocalAddress).IsEqualTo(server.ListeningChannel.LocalAddress);

            client.ClientChannel.SendMessage("hello");

            
           
        }
    }
}
