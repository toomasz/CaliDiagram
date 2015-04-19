using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NetworkModel.InProcNetwork
{
    public class NetworkClient
    {
        public NetworkClient(INetworkModel network,string address = null)
        {
            this.Network = network;

            _Channel = network.CreateClientSocket(address);
            _Channel.StateChanged += _Channel_StateChanged;
            reconnectTimer.Elapsed += reconnectTimer_Elapsed;
        }

        /// <summary>
        /// Number of connect attempts
        /// 0,1  - no attempt to reconnect, just try to connect once
        /// -1 - infinity reconnect attempts
        /// </summary>
        public int MaxConnectAttempts
        {
            get;
            set;
        }

        int failedCount = 0;
        void reconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            reconnectTimer.Stop();
            _Channel.RequestConnectionTo(RemoteAddress);
        }
        Timer reconnectTimer = new Timer();
        void _Channel_StateChanged(object sender, ConnectionState e)
        {
            switch (e)
            {
            case ConnectionState.Closed:
                ChangeStateTo(NetworkClientState.Closed);
                break;
            case ConnectionState.Closing:
                ChangeStateTo(NetworkClientState.Closed);
                break;
            case ConnectionState.Connecting:
                ChangeStateTo(NetworkClientState.Connecting);
                break;
            case ConnectionState.ConnectionFailed:
                ChangeStateTo(NetworkClientState.ConnectFailed);
                break;
            case ConnectionState.Established:
                ChangeStateTo(NetworkClientState.Connected);
                break;
            }
            if (e == ConnectionState.ConnectionFailed || e == ConnectionState.Closed)
            {
                failedCount++;

                if (!IsStarted)
                    return;
                if(MaxConnectAttempts != -1)
                {
                    if (failedCount >= MaxConnectAttempts)
                        return;
                }
              
                
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
        INetworkModel Network;

        INetworkSocket _Channel;
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
        public void StartConnectingTo(string remoteAddress)
        {
            this.RemoteAddress = remoteAddress;
            IsStarted = true;
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
            failedCount = 0;

            if (string.IsNullOrWhiteSpace(remoteAddress))
                ChangeStateTo(NetworkClientState.InvalidAddress);
            else
            {
                IsStarted = true;
                _Channel.RequestConnectionTo(RemoteAddress);
            }
        }

        void Stop()
        { 
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
