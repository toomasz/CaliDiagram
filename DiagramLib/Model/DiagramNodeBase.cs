using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramLib.Model
{
    /// <summary>
    /// Base diagram node base
    /// </summary>
    [DataContract]
    public class DiagramNodeBase
    {

        [DataMember]
        public Point Location { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
