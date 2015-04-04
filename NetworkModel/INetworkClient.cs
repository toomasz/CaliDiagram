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
        /// Request connection to remote address
        /// </summary>
        /// <param name="remoteAddress"></param>
        void RequestConnectionTo(string remoteAddress);

        /// <summary>
        /// Remote address
        /// </summary>
        string RemoteAddress { get; }
        /// <summary>
        /// Shutdown client
        /// </summary>
        void Close();
    }
}
