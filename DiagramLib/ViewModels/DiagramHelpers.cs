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
            double d = (Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            return d;
        }

        public static AttachSides GetAttachmentSidesForConnection(Rect fromRect, Rect toRect)
        {
            var fromMiddlePoint = new Point(fromRect.X + fromRect.Width / 2.0, fromRect.Y + fromRect.Height / 2.0);
            var toMiddlePoint = new Point(toRect.X + toRect.Width / 2.0, toRect.Y + toRect.Height / 2.0);
            const double Rad2Deg = 180.0 / Math.PI;
            double angle = Math.Atan2(fromMiddlePoint.X - toMiddlePoint.X, fromMiddlePoint.Y - toMiddlePoint.Y) * Rad2Deg +90;
            if (angle < 0)
                angle = 360 + angle;

            if(angle == 0)
                return new AttachSides(AttachDirection.Top, AttachDirection.Top);

            if (angle > 330 || angle < 45)
                return new AttachSides(AttachDirection.Right, AttachDirection.Left);
           // if (angle >= 30 && angle < 60)
          //      return new AttachSides(AttachDirection.Right, AttachDirection.Bottom);
            if (angle >= 45 && angle < 135)
                return new AttachSides(AttachDirection.Top, AttachDirection.Bottom);
            if (angle >= 120 && angle < 150)
                return new AttachSides(AttachDirection.Left, AttachDirection.Bottom);
            if (angle >= 150 && angle < 210)
                return new AttachSides(AttachDirection.Left, AttachDirection.Right);
            if (angle >= 210 && angle < 240)
                return new AttachSides(AttachDirection.Left, AttachDirection.Top);
            if (angle >= 240 && angle < 300)
                return new AttachSides(AttachDirection.Bottom, AttachDirection.Top);

            if (angle >= 300 && angle < 330)
                return new AttachSides(AttachDirection.Bottom, AttachDirection.Left);

            Console.WriteLine("angle = {0}", angle);
            return new AttachSides(AttachDirection.Top, AttachDirection.Top);
        }

        public static AttachSides GetAttachmentSidesForConnectionOld(Rect fromRect, Rect toRect)
        {
            var attachPointsFrom = AttachPoints(fromRect);
            var attachPointsTo = AttachPoints(toRect);

            var results = new List<Tuple<Point, Point, double, int, int>>();
            for (int i = 0; i < attachPointsFrom.Length; i++)
            {
                for (int j = 0; j < attachPointsTo.Length; j++)
                    results.Add(Tuple.Create(attachPointsFrom[i], attachPointsTo[j], DistanceBetweenPoints(attachPointsFrom[i], attachPointsTo[j]), i, j));
            }

            var bestMatch = results.OrderBy(r => r.Item3).First();

            return new AttachSides((AttachDirection)bestMatch.Item4, (AttachDirection)bestMatch.Item5);
        }
    }
}
