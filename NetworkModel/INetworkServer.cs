using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public interface INetworkServer
    {
        /// <summary>
        /// Start accepting connections
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool StartListening(string address);
        /// <summary>
        /// List of all client channels
        /// </summary>
        IList<IChannel> ClientChannels { get; }

        /// <summary>
        /// Listening channel
        /// </summary>
        IChannel ListeningChannel { get; }
        /// <summary>
        /// New client connected to server
        /// </summary>
        event EventHandler<IChannel> NewClientChannel;
        /// <summary>
        /// Connection to server was closed
        /// </summary>
        event EventHandler<IChannel> ClientChannelClosed;
    }
}
