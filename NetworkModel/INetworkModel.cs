using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public interface INetworkModel
    {
        INetworkClient CreateClient();
        INetworkServer CreateServer();
    }
}
