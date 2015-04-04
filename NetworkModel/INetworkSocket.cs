using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public interface INetworkSocket
    {
        /// <summary>
        /// Remote address which channel is connected to
        /// </summary>
        string RemoteAddress { get;  }

        /// <summary>
        /// Local address
        /// </summary>
        string LocalAddress { get; }

        /// <summary>
        /// Fires when new message arrives from remote party
        /// </summary>
        event EventHandler<object> MesageReceived;

        /// <summary>
        /// Queue message to be sent to remote part
        /// </summary>
        /// <param name="message"></param>
        /// <returns>false on failure</returns>
        bool SendMessage(object message);

        /// <summary>
        /// Request to close comm channel
        /// </summary>
        void Close();

        /// <summary>
        /// Type of channel(client/server)
        /// </summary>
        ChannelType Type { get; }

        /// <summary>
        /// State of connection
        /// </summary>
        ConnectionState State { get; }
    }
}
