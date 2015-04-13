using CaliDiagram.ViewModels;
using RaftDemo.Model;
using RaftDemo.NodeSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using NetworkModel;

namespace RaftDemo.ViewModels.Actors
{
    [ImplementPropertyChanged]
    public class ClientViewModel : ActorViewModel
    {
        RaftClient raftClient;
        public ClientViewModel(RaftClient raftClient)
            : base(raftClient)
        {
            Name = raftClient.Id;
            State = "Established";
            this.raftClient = raftClient;
            this.raftClient.Client.StateChanged += Client_StateChanged;
            ClientStateToView();
        }

        void Client_StateChanged(object sender, NetworkClientState e)
        {
            ClientStateToView();
        }


        void ClientStateToView()
        {
            NetworkClientState state = raftClient.Client.State;
            State = state.ToString();
            if (raftClient.Client.IsStarted)
                ButtonText = "Stop";
            else
                ButtonText = "Start";
        }
        public string State
        {
            get;
            set;
        }
        public string Address
        {
            get { return raftClient.ServerAddress; }
            set { raftClient.ServerAddress = value;}
        }
        public string ButtonText
        {
            get;
            set;
        }
        public void StartStop()
        {
            raftClient.Client.IsStarted = !raftClient.Client.IsStarted;
            ClientStateToView();
        }
    }
}
