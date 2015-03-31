using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public interface IChannel
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
        /// Type of channel
        /// </summary>
        ChannelType Type { get; }

        /// <summary>
        /// State of connection
        /// </summary>
        ConnectionState State { get; }
    }
}
