using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NetworkModel.InProcNetwork
{
    public class InProcClient: INetworkClient
    {
        public InProcClient(InProcNetwork network,string address = null)
        {
            this.Network = network;
            if (address == null)
                address = Network.GetNextClientSocketAddress();

            _Channel = new InProcSocket(network, ChannelType.Client) { LocalAddress = address };
            _Channel.StateChanged += _Channel_StateChanged;
            reconnectTimer.Elapsed += reconnectTimer_Elapsed;
        }
        int failedCount = 0;
        void reconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            reconnectTimer.Stop();
            Network.RequestClientConnectioTo(_Channel, RemoteAddress);
        }
        Timer reconnectTimer = new Timer();
        void _Channel_StateChanged(object sender, ConnectionState e)
        {
            if (e == ConnectionState.ConnectionFailed)
            {
                if (!IsStarted)
                    return;

                failedCount++;
                if (failedCount < 4)
                    reconnectTimer.Interval = 400;
                else if (failedCount < 8)
                    reconnectTimer.Interval = 800;
                else if (failedCount < 16)
                    reconnectTimer.Interval = 1800;
                else
                    reconnectTimer.Interval = 4000;

                reconnectTimer.Start();
            }
        }
        InProcNetwork Network;

        InProcSocket _Channel;
        public INetworkSocket ClientChannel
        {
            get
            {
                return _Channel;
            }
        }
        bool _IsStarted = false;
        public bool IsStarted
        {
            get
            {
                return _IsStarted;
            }
            set
            {
                if (_IsStarted != value)
                {
                    _IsStarted = value;
                    HandleStartStop(_IsStarted);
                }
            }
        }
        void HandleStartStop(bool isStarted)
        {
            if (isStarted)
                StartConnection(RemoteAddress);
            else
                Stop();
        }

        void StartConnection(string remoteAddress)
        {
            if (string.IsNullOrWhiteSpace(remoteAddress))
                ChangeStateTo(NetworkClientState.InvalidAddress);
            else
            {
                IsStarted = true;
                Network.RequestClientConnectioTo(_Channel, remoteAddress);
            }
        }

        void Stop()
        {
            if (!IsStarted)
                throw new Exception("Client is already stopped");
            IsStarted = false;
            reconnectTimer.Stop();
            if(_Channel.State != ConnectionState.Closed)
                _Channel.Close();
        }

        public string LocalAddress
        {
            get
            {
                return ClientChannel.LocalAddress;
            }
        }

        public string RemoteAddress
        {
            get;
            set;
        }

        void ChangeStateTo(NetworkClientState newState)
        {
            var oldState = State;
            State = newState;
            if(oldState != newState)
            {
                if (StateChanged != null)
                    StateChanged(this, newState);
            }
        }

        public NetworkClientState State
        {
            get;
            private set;
        }

        public event EventHandler<NetworkClientState> StateChanged;
    }
}
