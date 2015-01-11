using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib
{
    using ViewModels;
    public abstract class DiagramDefinitionBase
    {
        public DiagramDefinitionBase()
        {
            ConnectorSideStrategy = new VerticalFavourizedConnectionSrategy();
        }
        public abstract ConnectionViewModel CreateConnection(NodeBaseViewModel from, NodeBaseViewModel to);
        public IConnectorSideStrategy ConnectorSideStrategy {get; set;}
    }
}
