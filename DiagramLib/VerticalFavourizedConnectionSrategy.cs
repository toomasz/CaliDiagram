using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramLib
{
    /// <summary>
    /// Strategy that favourizes vertical connections(Top-bottom, bottom-top)
    /// It's best for wide rectangles nodes 
    /// </summary>
    public class VerticalFavourizedConnectionSrategy: IConnectorSideStrategy
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
            Debug.Assert(!connection.From.Rect.IsEmpty, "Rectangle is empty");
            Debug.Assert(!connection.From.Rect.IsEmpty, "Rectangle is empty");
            return GetAttachmentSidesForConnection(connection.From.Rect, connection.To.Rect);
        }
    }
}
