using CaliDiagram.ViewModels;

namespace CaliDiagram
{
    /// <summary>
    /// Connector placement behaviour
    /// </summary>
    public interface IConnectorSideStrategy
    {
        AttachSides GetSidesForConnection(ConnectionViewModel connection);
    }
}
