using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramLib.ViewModels
{
    public class DiagramHelpers
    {
        public static Point GetAttachmentLocation(NodeBaseViewModel control, Point fromPoint, AttachDirection pos)
        {
            switch (pos)
            {
                case AttachDirection.Top:
                    return new Point(fromPoint.X - control.Size.Width / 2, fromPoint.Y - control.Size.Height/2);
                case AttachDirection.Right:
                    return new Point(fromPoint.X - control.Size.Width / 2 , fromPoint.Y - (control.Size.Height / 2));
                case AttachDirection.Bottom:
                    return new Point(fromPoint.X - control.Size.Width / 2, fromPoint.Y - control.Size.Height / 2);
                case AttachDirection.Left:
                    return new Point(fromPoint.X - control.Size.Width/2, fromPoint.Y - control.Size.Height / 2);
                default:
                    throw new ArgumentException();
            }

        }
        public static Point GetAttachmentLocationOld(NodeBaseViewModel control, Point fromPoint, AttachDirection pos)
        {
            switch (pos)
            {
                case AttachDirection.Top:
                    return new Point(fromPoint.X - control.Size.Width / 2, fromPoint.Y - control.Size.Height);
                case AttachDirection.Right:
                    return new Point(fromPoint.X, fromPoint.Y - (control.Size.Height / 2));
                case AttachDirection.Bottom:
                    return new Point(fromPoint.X - control.Size.Width / 2, fromPoint.Y);
                case AttachDirection.Left:
                    return new Point(fromPoint.X - control.Size.Width, fromPoint.Y - control.Size.Height / 2);
                default:
                    throw new ArgumentException();
            }

        }

        public static Point GetMiddlePoint(AttachDirection dir, Rect rec)
        {
            if (dir == AttachDirection.Top)
                return new Point(rec.X + rec.Width / 2, rec.Y);
            if (dir == AttachDirection.Right)
                return new Point(rec.X + rec.Width, rec.Y + rec.Height / 2);
            if (dir == AttachDirection.Bottom)
                return new Point(rec.X + rec.Width / 2, rec.Y + rec.Height);
            if (dir == AttachDirection.Left)
                return new Point(rec.X, rec.Y + rec.Height / 2);
            throw new ArgumentException();
        }

        public static Point[] AttachPoints(Rect rect)
        {
            return new Point[]
            {
                DiagramHelpers.GetMiddlePoint(AttachDirection.Top, rect),
                DiagramHelpers.GetMiddlePoint(AttachDirection.Right, rect),
                DiagramHelpers.GetMiddlePoint(AttachDirection.Bottom, rect),
                DiagramHelpers.GetMiddlePoint(AttachDirection.Left, rect)
            };

        }

        public static double DistanceBetweenPoints(Point a, Point b)
        {
            return (Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public static double GetAngleBetweenRects(Rect fromRect, Rect toRect)
        {
            var fromMiddlePoint = new Point(fromRect.X + fromRect.Width / 2.0, fromRect.Y + fromRect.Height / 2.0);
            var toMiddlePoint = new Point(toRect.X + toRect.Width / 2.0, toRect.Y + toRect.Height / 2.0);
            const double Rad2Deg = 180.0 / Math.PI;
            double angle = Math.Atan2(fromMiddlePoint.X - toMiddlePoint.X, fromMiddlePoint.Y - toMiddlePoint.Y) * Rad2Deg + 90;
            if (angle < 0)
                angle = 360 + angle;
            return angle;
        }
    }
}
