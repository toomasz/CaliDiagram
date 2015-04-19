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
    public class BasicActorTest
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
                Check.That(clientActor.Clients[0].Client.State).IsEqualTo(NetworkClientState.Connected);

                Check.That(network.ConnectedSocketCount).IsEqualTo(2);

                // add another connection to server
                clientActor.RequestConnectionTo(serverAddress);

                Wait(15);

                Check.That(clientActor.WorkingClientCount).IsEqualTo(2);
                Check.That(clientActor.Clients[0].Client.State).IsEqualTo(NetworkClientState.Connected);
                Check.That(clientActor.Clients[1].Client.State).IsEqualTo(NetworkClientState.Connected);
            }
        }
    }
}
