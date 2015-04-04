using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public interface INetworkModel
    {
        /// <summary>
        /// Create network client
        /// </summary>
        /// <param name="clientAddress">Address of client, if null will be assigned by network</param>
        /// <returns></returns>
        INetworkClient CreateClient(string socketAddress = null);

        /// <summary>
        /// Create network server
        /// </summary>
        /// <returns></returns>
        INetworkServer CreateServer(string socketAddress);
    }
}
