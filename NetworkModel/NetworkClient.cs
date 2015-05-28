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
            
            reconnectTimer.Elapsed += reconnectTimer_Elapsed;
            MaxConnectAttempts = 1;
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

        private void _Channel_StateChanged(object sender, ConnectionState e)
        {
            switch (e)
            {
                case ConnectionState.Connecting:
                    ChangeStateTo(NetworkClientState.Connecting);
                    break;
                case ConnectionState.Established:
                    ChangeStateTo(NetworkClientState.Connected);
                    RaiseConnectionStateChaned(true);
                    break;
                case  ConnectionState.Closing:
                    RaiseConnectionStateChaned(false);
                    break;
                case ConnectionState.Closed:
                {
                    RaiseConnectionStateChaned(false);
                    failedCount++;
                    if (MaxConnectAttempts != -1)
                    {
                        if (failedCount >= MaxConnectAttempts)
                        {
                            ChangeStateTo(NetworkClientState.Disconnected);
                            return;
                        }
                    }
                    ChangeStateTo(NetworkClientState.Reconnecting);

                    if (failedCount < 4)
                        reconnectTimer.Interval = 400;
                    else if (failedCount < 8)
                        reconnectTimer.Interval = 800;
                    else if (failedCount < 16)
                        reconnectTimer.Interval = 1800;
                    else
                        reconnectTimer.Interval = 4000;

                    reconnectTimer.Start();
                    break;
                }
                case ConnectionState.ConnectionFailed:
                {
                    RaiseConnectionStateChaned(false);
                    failedCount++;

                    if (!IsStarted)
                        return;

                    if (MaxConnectAttempts != -1)
                    {
                        if (failedCount >= MaxConnectAttempts)
                        {
                            ChangeStateTo(NetworkClientState.ConnectFailed);
                            return;
                        }
                    }
                    ChangeStateTo(NetworkClientState.Reconnecting);

                    if (failedCount < 4)
                        reconnectTimer.Interval = 400;
                    else if (failedCount < 8)
                        reconnectTimer.Interval = 800;
                    else if (failedCount < 16)
                        reconnectTimer.Interval = 1800;
                    else
                        reconnectTimer.Interval = 4000;

                    reconnectTimer.Start();
                    break;
                }
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
            _Channel.StateChanged += _Channel_StateChanged;
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
            _Channel.StateChanged -= _Channel_StateChanged;
            if(_Channel.State != ConnectionState.Closed)
                _Channel.Close();
            ChangeStateTo(NetworkClientState.Stopped);
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

        public bool IsEstablished { get; private set; }
        void RaiseConnectionStateChaned(bool isEstablished)
        {
            if (IsEstablished != isEstablished)
            {
                IsEstablished = isEstablished;
                var connectionStateChanged = ConnectionStateChanged;
                if (connectionStateChanged != null)
                    connectionStateChanged(this, isEstablished);
            }
        }
        public event EventHandler<NetworkClientState> StateChanged;
        /// <summary>
        /// Fired when connection is established/closed
        /// true-established
        /// false-closed
        /// </summary>
        public event EventHandler<bool> ConnectionStateChanged;
    }
}
