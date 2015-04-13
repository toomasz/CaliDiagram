using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkModel;
using NetworkModel.InProcNetwork;
using NFluent;

namespace NetworkTest
{
    [TestClass]
    public class InProcNetworkTest
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

        void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        [TestMethod]
        public void ClientServerConnectionTest()
        {
            using (InProcNetwork network1 = new InProcNetwork() {ConnectionEstablishLatency = 30 })
            {

                INetworkServer server = network1.CreateServer("server");
                Check.That(server.ListeningChannel.LocalAddress).IsEqualTo("server");
                Check.That(server.ListeningChannel.RemoteAddress).IsNull();

                INetworkClient client = network1.CreateClient("c1");
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Closed);

                // Request connection to server
                client.RemoteAddress = "server";
                client.IsStarted = true;

                // Test client socket state after connect request
                Check.That(client.ClientChannel.RemoteAddress).IsEqualTo("server");
                Check.That(client.ClientChannel.Type).IsEqualTo(ChannelType.Client);
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Connecting);
                // wait for connection to be established
                Wait(35);

                // check that connection is established
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Established);
                Check.That(server.ClientChannels.Count).IsEqualTo(1);
                var serverSideChannel = server.ClientChannels[0];
                Check.That(serverSideChannel.LocalAddress).IsEqualTo(server.ListeningChannel.LocalAddress);
                Check.That(serverSideChannel.RemoteAddress).IsEqualTo(client.ClientChannel.LocalAddress);
                Check.That(serverSideChannel.State).IsEqualTo(ConnectionState.Established);

                client.ClientChannel.Close();
                serverSideChannel.Close();
            }           
        }
        void TestNetworkState(INetworkModel network)
        {
            InProcNetwork inProcNetwork = network as InProcNetwork;
            if(inProcNetwork != null)
            {
                Check.That(inProcNetwork.TaskScheduler.Exceptions.Count).IsEqualTo(0);
            }
        }
        [TestMethod]
        public void ConnectionCloseFromClientSide()
        {
            using (InProcNetwork network1 = new InProcNetwork() {ConnectionEstablishLatency = 10, ConnectionCloseLatency = 10})
            {
                var server = network1.CreateServer("127.0.0.0:80");
                var client = network1.CreateClient("127.0.0.0:5341");
                Check.That(network1.ListeningSocketCount).IsEqualTo(1);

                // Connect client to server
                client.RemoteAddress = "127.0.0.0:80";
                client.IsStarted = true;
                Wait(13);

                // Check that connection is established on both sides
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Established);
                Check.That(server.ClientChannels.Count).IsEqualTo(1);
                var serverSideChannel = server.ClientChannels[0];
                Check.That(serverSideChannel.State).IsEqualTo(ConnectionState.Established);

                Check.That(network1.CommunicationSocketCount).IsEqualTo(2);
                TestNetworkState(network1);
                // Close connection from client side
                client.IsStarted = false;

                Wait(20);
                TestNetworkState(network1);
               // Check.That(server.ClientChannels[0].State).IsEqualTo(ConnectionState.Closed);
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Closed);
                Check.That(server.ClientChannels.Count).IsEqualTo(0);
                Check.That(network1.CommunicationSocketCount).IsEqualTo(0);
            }
        }

        [TestMethod]
        public void ConnectionCloseFromServerSide()
        {
            using (InProcNetwork network1 = new InProcNetwork() { ConnectionEstablishLatency = 10, ConnectionCloseLatency = 10 })
            {
                var server = network1.CreateServer("127.0.0.0:80");
                var client = network1.CreateClient("127.0.0.0:5341");
                Check.That(network1.ListeningSocketCount).IsEqualTo(1);

                // Connect client to server
                client.RemoteAddress = "127.0.0.0:80";
                client.IsStarted = true;
                Wait(13);

                // Check that connection is established on both sides
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Established);
                Check.That(server.ClientChannels.Count).IsEqualTo(1);
                var serverSideChannel = server.ClientChannels[0];
                Check.That(serverSideChannel.State).IsEqualTo(ConnectionState.Established);

                Check.That(network1.CommunicationSocketCount).IsEqualTo(2);
                TestNetworkState(network1);
                // Close connection from client side
                serverSideChannel.Close();

                Wait(20);
                TestNetworkState(network1);
                // Check.That(server.ClientChannels[0].State).IsEqualTo(ConnectionState.Closed);
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Closed);
                Check.That(server.ClientChannels.Count).IsEqualTo(0);
                Check.That(network1.CommunicationSocketCount).IsEqualTo(0);
            }
        }

        [TestMethod]
        public void TestMultipleClientFromOneAddress()
        {
            using (InProcNetwork network = new InProcNetwork() 
            { 
                ConnectionEstablishLatency = 10, 
                ConnectionCloseLatency = 10,
                ConnectionDefaultLatency = 10
            })
            {
            //    var client1 = network.CreateClient();
           //     var client2 = network.CreateClient();
            }

        }

        [TestMethod]
        public void MessageSendingAndReceiving()
        {
            using (InProcNetwork network1 = new InProcNetwork() 
            { 
                ConnectionEstablishLatency = 10, 
                ConnectionCloseLatency = 10,
                ConnectionDefaultLatency = 10
            })
            {
                var server = network1.CreateServer("127.0.0.0:80");
                List<MessageReceivedArgs> receivedMessages = new List<MessageReceivedArgs>();

                server.MessageReceived += (o,args) =>
                {
                    receivedMessages.Add(args);
                    args.Socket.SendMessage("response");
                };
                var client1 = network1.CreateClient("127.0.0.0:1");
                var client2 = network1.CreateClient("127.0.0.0:2");
                var client3 = network1.CreateClient("127.0.0.0:3");
                List<object> messagesReceivedByClients = new List<object>();
                client1.ClientChannel.MesageReceived += (o,m) => { messagesReceivedByClients.Add(m); };
                client2.ClientChannel.MesageReceived += (o,m) => { messagesReceivedByClients.Add(m); };
                client3.ClientChannel.MesageReceived += (o,m) => { messagesReceivedByClients.Add(m); };

                client1.RemoteAddress = server.ListeningChannel.LocalAddress;
                client1.IsStarted = true;

                client2.RemoteAddress = server.ListeningChannel.LocalAddress;
                client2.IsStarted = true;

                client3.RemoteAddress = server.ListeningChannel.LocalAddress;
                client3.IsStarted = true;


                // wait for connection to be established
                Thread.Sleep(12);
                client1.ClientChannel.SendMessage("message1");
                client2.ClientChannel.SendMessage("message2");
                client3.ClientChannel.SendMessage("message3");
                // wait for messages to arrive to server
                Thread.Sleep(110);
                Check.That(receivedMessages.Count).IsEqualTo(3);
                Check.That(messagesReceivedByClients.Count).IsEqualTo(3);
            }


        }
    }
}
