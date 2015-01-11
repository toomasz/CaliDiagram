using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib.ViewModels
{
    public class AttachSides
    {
        public AttachSides(AttachDirection fromSide, AttachDirection toSide)
        {
            FromSide = fromSide;
            ToSide = toSide;
        }
        public AttachDirection FromSide
        {
            get;
            private set;
        }
        public AttachDirection ToSide
        {
            get;
            private set;
        }
    }
}
