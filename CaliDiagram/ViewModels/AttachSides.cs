
namespace CaliDiagram.ViewModels
{
    /// <summary>
    /// Represents side pair used to determine connection attach points sides
    /// </summary>
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
