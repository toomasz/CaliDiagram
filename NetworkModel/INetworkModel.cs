using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public interface INetworkModel : IDisposable
    {
        /// <summary>
        /// Create network client
        /// </summary>
        /// <param name="clientAddress">Address of client, if null will be assigned by network</param>
        /// <returns></returns>
        INetworkSocket CreateClientSocket(string socketAddress = null);

        /// <summary>
        /// Create network server
        /// </summary>
        /// <returns></returns>
        INetworkServer CreateServer(string socketAddress, bool startListening = true);

        /// <summary>
        /// Number of listening sockets in network model instance
        /// </summary>
        int ListeningSocketCount { get; }

        /// <summary>
        /// Number of connected sockets in network model instance
        /// </summary>
        int ConnectedSocketCount { get; }

        /// <summary>
        /// Exceptions thrown from other threads
        /// </summary>
        List<Exception> Exceptions { get; } 
    }
}
