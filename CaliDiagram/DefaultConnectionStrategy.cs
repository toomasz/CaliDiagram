using CaliDiagram.ViewModels;
using System.Diagnostics;
using System.Windows;

namespace CaliDiagram
{
    /// <summary>
    /// Strategy that favourizes vertical connections(Top-bottom, bottom-top)
    /// It's best for wide rectangles nodes 
    /// </summary>
    public class DefaultConnectionStrategy: IConnectorSideStrategy
    {
        public static AttachSides GetAttachmentSidesForConnection(Rect fromRect, Rect toRect)
        {
            double angle = DiagramHelpers.GetAngleBetweenRects(fromRect, toRect);

            if (angle > 320 || angle < 40)
                return new AttachSides(AttachDirection.Right, AttachDirection.Left);
            if (angle >= 40 && angle < 140)
                return new AttachSides(AttachDirection.Top, AttachDirection.Bottom);
            if (angle >= 140 && angle < 220)
                return new AttachSides(AttachDirection.Left, AttachDirection.Right);
            if (angle >= 220)
                return new AttachSides(AttachDirection.Bottom, AttachDirection.Top);

            return new AttachSides(AttachDirection.Top, AttachDirection.Top);
        }

        public AttachSides GetSidesForConnection(ConnectionViewModel connection)
        {
            Debug.Assert(connection.From != null, "From connection is null");
            Debug.Assert(connection.To != null, "To connection is null");
            Debug.Assert(!connection.From.Rect.IsEmpty, "From Rectangle is empty");
            Debug.Assert(!connection.To.Rect.IsEmpty, "To Rectangle is empty");
            return GetAttachmentSidesForConnection(connection.From.Rect, connection.To.Rect);
        }
    }
}
