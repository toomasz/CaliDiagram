using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib.Model
{
    [DataContract]
    public class DiagramModel
    {
        public DiagramModel()
        {
            Nodes = new List<DiagramNodeBase>();
            Edges = new List<DiagramConnection>();
        }
        [DataMember]
        public List<DiagramNodeBase> Nodes { get; private set; }
        [DataMember]
        public List<DiagramConnection> Edges { get; private set; } 
    }
}
