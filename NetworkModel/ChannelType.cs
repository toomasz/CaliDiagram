using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public enum ChannelType
    {
        /// <summary>
        /// NetworkClient side channel
        /// </summary>
        Client,
        /// <summary>
        /// Server side channel
        /// </summary>
        Server,
        /// <summary>
        /// Listening channel
        /// </summary>
        Listening
    }
}
