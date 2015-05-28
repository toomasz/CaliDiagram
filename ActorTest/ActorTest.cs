using System;
using System.ComponentModel.Design;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkModel;
using NetworkModel.Actors.ActorEvents;
using NetworkModel.InProcNetwork;
using NFluent;
using System.Collections.Generic;

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
                clientActor.AddConnectionTo(serverAddress);

                // client actor should not create any clients when not started
                Check.That(clientActor.WorkingClientCount).IsEqualTo(0);
                // start server and client actor
                serverActor.Start();
                clientActor.Start();
                
                // wait for connection to be established
                Wait(15);

                Check.That(clientActor.WorkingClientCount).IsEqualTo(1);
                Check.That(clientActor.NetworkClientContexts[0].NetworkClient.State).IsEqualTo(NetworkClientState.Connected);

                Check.That(network.ConnectedSocketCount).IsEqualTo(2);

                // add another connection to server
                clientActor.AddConnectionTo(serverAddress);

                Wait(15);
                Check.That(network.ConnectedSocketCount).IsEqualTo(4);

                Check.That(clientActor.WorkingClientCount).IsEqualTo(2);
                Check.That(clientActor.NetworkClientContexts[0].NetworkClient.State).IsEqualTo(NetworkClientState.Connected);
                Check.That(clientActor.NetworkClientContexts[1].NetworkClient.State).IsEqualTo(NetworkClientState.Connected);

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
                clientActor.AddConnectionTo(serverAddress);

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
                string serverAddress = "127.0.0.1:99";
                // setup client and server actors
                TestClientActor clientActor1 = new TestClientActor(network);
                TestClientActor clientActor2 = new TestClientActor(network);
                TestServerActor serverActor = new TestServerActor(network, serverAddress);

                clientActor1.AddConnectionTo(serverAddress);
                clientActor2.AddConnectionTo(serverAddress);

                clientActor1.Start();
                clientActor2.Start();
                serverActor.Start();

                Wait(15);
                Check.That(network.ConnectedSocketCount).IsEqualTo(4);
                Check.That(clientActor1.WorkingClientCount).IsEqualTo(1);
                Check.That(clientActor2.WorkingClientCount).IsEqualTo(1);
                // stop server actor
                serverActor.Stop();
              
                Wait(15);
                // no connected socket should be present in network mode now
                Check.That(network.ConnectedSocketCount).IsEqualTo(0);

                // clients should still have one working client each
                Check.That(clientActor1.WorkingClientCount).IsEqualTo(1);
                Check.That(clientActor2.WorkingClientCount).IsEqualTo(1);

                Check.That(clientActor1.NetworkClientContexts[0].NetworkClient.State).IsEqualTo(NetworkClientState.Reconnecting);
                Check.That(clientActor2.NetworkClientContexts[0].NetworkClient.State).IsEqualTo(NetworkClientState.Reconnecting);
            }
        }

        [TestMethod]
        public void TestActorEvents()
        {
            using (INetworkModel network = CreateNetworkModel())
            {
                string serverAddress = "127.0.0.1:777";

                // setup client and server actors
                TestClientActor clientActor1 = new TestClientActor(network);
                clientActor1.AddConnectionTo(serverAddress);

                
                TestServerActor serverActor = new TestServerActor(network, serverAddress);

                
                List<ActorEventBase> serverActorEvents = new List<ActorEventBase>();
                serverActor.ActorEvent += (sender, ev) =>
                {
                    serverActorEvents.Add(ev);
                };
                

                
                // this starts clientActor event loop
                clientActor1.Start();
                // this starts serverActor event loop
                serverActor.Start();
                
                // wait for connections to be established
                Wait(100);

                Check.That(clientActor1.WorkingClientCount).IsEqualTo(1);

                Check.That(serverActorEvents.Count).IsEqualTo(1);
                Check.That(serverActorEvents[0]).IsInstanceOf<ChannelAddedEvent>();

            }
        }
    }
}
