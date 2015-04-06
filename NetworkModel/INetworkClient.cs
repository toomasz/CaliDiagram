using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
   
    public interface INetworkClient
    {
        /// <summary>
        /// Channel that client uses to send messages to server
        /// </summary>
        INetworkSocket ClientChannel { get;}

        /// <summary>
        /// Remote address
        /// </summary>
        string RemoteAddress { get; set; }
        /// <summary>
        /// State of client(connected, etc)
        /// </summary>
        NetworkClientState State { get; }
        /// <summary>
        /// Fired when state changes
        /// </summary>
        event EventHandler<NetworkClientState> StateChanged;
        /// <summary>
        /// Used to control client
        /// </summary>
        bool IsStarted { get; set; }
    }
}
