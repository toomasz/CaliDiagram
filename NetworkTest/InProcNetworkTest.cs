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

                NetworkClient client = new NetworkClient(network1, "c1");
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
            using (InProcNetwork network1 = new InProcNetwork 
            {
                ConnectionEstablishLatency = 10, 
                ConnectionCloseLatency = 10
            })
            {
                var server = network1.CreateServer("127.0.0.0:80");
                var client = new NetworkClient(network1,"127.0.0.0:5341");
                Check.That(network1.ListeningSocketCount).IsEqualTo(1);

                // Connect client to server   
                client.StartConnectingTo("127.0.0.0:80");
              
                Wait(20);

                // Check that connection is established on both sides
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Established);
                Check.That(server.ClientChannels.Count).IsEqualTo(1);
                var serverSideChannel = server.ClientChannels[0];
                Check.That(serverSideChannel.State).IsEqualTo(ConnectionState.Established);

                Check.That(network1.ConnectedSocketCount).IsEqualTo(2);
                TestNetworkState(network1);
                // Close connection from client side
                client.IsStarted = false;

                Wait(20);
                TestNetworkState(network1);
               // Check.That(server.ClientChannels[0].State).IsEqualTo(ConnectionState.Closed);
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Closed);
                Check.That(server.ClientChannels.Count).IsEqualTo(0);
                Check.That(network1.ConnectedSocketCount).IsEqualTo(0);
            }
        }

        [TestMethod]
        public void ConnectionCloseFromServerSide()
        {
            using (InProcNetwork network1 = new InProcNetwork() { ConnectionEstablishLatency = 10, ConnectionCloseLatency = 10 })
            {
                var server = network1.CreateServer("127.0.0.0:80");
                var client = new NetworkClient(network1,"127.0.0.0:5341");
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

                Check.That(network1.ConnectedSocketCount).IsEqualTo(2);
                TestNetworkState(network1);
                // Close connection from client side
                serverSideChannel.Close();

                Wait(20);
                TestNetworkState(network1);
                // Check.That(server.ClientChannels[0].State).IsEqualTo(ConnectionState.Closed);
                Check.That(client.ClientChannel.State).IsEqualTo(ConnectionState.Closed);
                Check.That(server.ClientChannels.Count).IsEqualTo(0);
                Check.That(network1.ConnectedSocketCount).IsEqualTo(0);
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
                string serverAddress = "127.0.0.1:90";

                var server = network.CreateServer(serverAddress);
                var client1 = new NetworkClient(network);
                var client2 = new NetworkClient(network);

                client1.StartConnectingTo(serverAddress);
                client2.StartConnectingTo(serverAddress);

                Wait(20);

                Check.That(network.ConnectedSocketCount).IsEqualTo(4);

                server.Dispose();

                Wait(20);

                Check.That(client1.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(client2.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(network.ConnectedSocketCount).IsEqualTo(0);
                

            }

        }

        [TestMethod]
        public void TestServerRestart()
        {
            using (InProcNetwork network = new InProcNetwork()
            {
                ConnectionEstablishLatency = 10,
                ConnectionCloseLatency = 10,
                ConnectionDefaultLatency = 10
            })
            {
                string serverAddress = "127.0.0.1:90";

                var server = network.CreateServer(serverAddress);
                var client1 = new NetworkClient(network) { MaxConnectAttempts = 1 };
                var client2 = new NetworkClient(network) { MaxConnectAttempts = 1 };
                var client3 = new NetworkClient(network) { MaxConnectAttempts = 1 };

                client1.StartConnectingTo(serverAddress);
                client2.StartConnectingTo(serverAddress);
                client3.StartConnectingTo(serverAddress);

                Wait(20);

                Check.That(network.ConnectedSocketCount).IsEqualTo(6);

                server.Stop();

                Wait(20);

                Check.That(client1.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(client2.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(client3.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(network.ConnectedSocketCount).IsEqualTo(0);

                Wait(500);

                Check.That(client1.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(client2.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(client3.State).IsEqualTo(NetworkClientState.Closed);
                Check.That(network.ConnectedSocketCount).IsEqualTo(0);

                server.StartListening(serverAddress);
                client1.IsStarted = false;
                client2.IsStarted = false;
                client3.IsStarted = false;

                client1.StartConnectingTo(serverAddress);
                client2.StartConnectingTo(serverAddress);
                client3.StartConnectingTo(serverAddress);

                Wait(20);

                Check.That(network.ConnectedSocketCount).IsEqualTo(6);
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
                var client1 = new NetworkClient(network1,"127.0.0.0:1");
                var client2 = new NetworkClient(network1,"127.0.0.0:2");
                var client3 = new NetworkClient(network1, "127.0.0.0:3");
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
