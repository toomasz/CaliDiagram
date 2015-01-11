using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib
{
    /// <summary>
    /// Connector placement behaviour
    /// </summary>
    public interface IConnectorSideStrategy
    {
        AttachSides GetSidesForConnection(ConnectionViewModel connection);
    }
}
