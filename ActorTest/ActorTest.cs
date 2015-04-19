using System;
using System.ComponentModel.Design;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkModel;
using NetworkModel.InProcNetwork;
using NFluent;

namespace ActorTest
{
    [TestClass]
    public class ActorTest
    {
        INetworkModel CreateNetworkModel()
        {
            return new InProcNetwork
            {
                ConnectionCloseLatency = 10,
                ConnectionEstablishLatency = 10,
                ConnectionDefaultLatency = 10
            };
        }

        void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        [TestMethod]
        public void TestClientServerActor()
        {
            using (INetworkModel network = CreateNetworkModel())
            {
                // setup client and server actors
                TestClientActor clientActor = new TestClientActor(network);
                string serverAddress = "127.0.0.1:99";
                TestServerActor serverActor = new TestServerActor(network, serverAddress);
                clientActor.RequestConnectionTo(serverAddress);

                // client actor should not create any clients when not started
                Check.That(clientActor.WorkingClientCount).IsEqualTo(0);
                // start server and client actor
                serverActor.Start();
                clientActor.Start();
                
                // wait for connection to be established
                Wait(15);

                Check.That(clientActor.WorkingClientCount).IsEqualTo(1);
                Check.That(clientActor.Clients[0].NetworkClient.State).IsEqualTo(NetworkClientState.Connected);

                Check.That(network.ConnectedSocketCount).IsEqualTo(2);

                // add another connection to server
                clientActor.RequestConnectionTo(serverAddress);

                Wait(15);
                Check.That(network.ConnectedSocketCount).IsEqualTo(4);

                Check.That(clientActor.WorkingClientCount).IsEqualTo(2);
                Check.That(clientActor.Clients[0].NetworkClient.State).IsEqualTo(NetworkClientState.Connected);
                Check.That(clientActor.Clients[1].NetworkClient.State).IsEqualTo(NetworkClientState.Connected);

                // stop client actor
                clientActor.Stop();

                Wait(15);
            }
        }

        [TestMethod]
        public void ClientRestartTest()
        {
            using (INetworkModel network = CreateNetworkModel())
            {
                // setup client and server actors
                TestClientActor clientActor = new TestClientActor(network);
                string serverAddress = "127.0.0.1:99";
                TestServerActor serverActor = new TestServerActor(network, serverAddress);
                clientActor.RequestConnectionTo(serverAddress);

                clientActor.Start();
                serverActor.Start();

                Wait(15);
                Check.That(network.ConnectedSocketCount).IsEqualTo(2);

                clientActor.Stop();
                Wait(15);
                Check.That(network.ConnectedSocketCount).IsEqualTo(0);
            }
        }

        [TestMethod]
        public void ServerActorShutdownTest()
        {
            using (INetworkModel network = CreateNetworkModel())
            {
                // setup client and server actors
                TestClientActor clientActor1 = new TestClientActor(network);
                TestClientActor clientActor2 = new TestClientActor(network);

                string serverAddress = "127.0.0.1:99";
                TestServerActor serverActor = new TestServerActor(network, serverAddress);
                clientActor1.RequestConnectionTo(serverAddress);
                clientActor2.RequestConnectionTo(serverAddress);

                clientActor1.Start();
                clientActor2.Start();
                serverActor.Start();

                Wait(15);
                Check.That(network.ConnectedSocketCount).IsEqualTo(4);

                // stop server actor
                serverActor.Stop();
              
                Wait(15);
                // no connected socket should be present in network mode now
                Check.That(network.ConnectedSocketCount).IsEqualTo(0);

                // clients should still have one working client each
                Check.That(clientActor1.WorkingClientCount).IsEqualTo(1);
                Check.That(clientActor2.WorkingClientCount).IsEqualTo(1);

                Check.That(clientActor1.Clients[0].NetworkClient.State).IsEqualTo(NetworkClientState.Reconnecting);
                Check.That(clientActor2.Clients[0].NetworkClient.State).IsEqualTo(NetworkClientState.Reconnecting);
            }
        }
    }
}
