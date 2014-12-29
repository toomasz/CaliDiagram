using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib
{
    using ViewModels;
    public interface IDiagramDefinition
    {
        ConnectionViewModel CreateConnection(DiagramBaseViewModel from, DiagramBaseViewModel to);
    }
}
