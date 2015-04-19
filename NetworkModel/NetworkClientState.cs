using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    /// <summary>
    /// States for NetworkClient
    /// </summary>
    public enum NetworkClientState
    {
        /// <summary>
        /// Client is stopped(not connected nor trying to connect)
        /// </summary>
        Stopped,
        /// <summary>
        /// Invalid address was supplied to client
        /// </summary>
        InvalidAddress,
        /// <summary>
        /// Resolving address of remote server
        /// </summary>
        ResolvingName,
        /// <summary>
        /// Connecting to remote server
        /// </summary>
        Connecting,
        /// <summary>
        /// Connection is established
        /// </summary>
        Connected,
        /// <summary>
        /// Client is reconnecting(due to failed connection attempt or server side connection close)
        /// </summary>
        Reconnecting,
        /// <summary>
        /// Failed to connect
        /// </summary>
        ConnectFailed,
        /// <summary>
        /// Disconnecting
        /// </summary>
        Disconnecting
    }
}
