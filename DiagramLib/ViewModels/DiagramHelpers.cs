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
        public static Point GetAttachmentLocation(DiagramBaseViewModel control, Point fromPoint, AttachDirection pos)
        {
            switch (pos)
            {
                case AttachDirection.Top:
                    return new Point(fromPoint.X - control.Width / 2, fromPoint.Y - control.Height);
                case AttachDirection.Right:
                    return new Point(fromPoint.X, fromPoint.Y - (control.Height / 2));
                case AttachDirection.Bottom:
                    return new Point(fromPoint.X - control.Width / 2, fromPoint.Y);
                case AttachDirection.Left:
                    return new Point(fromPoint.X - control.Width, fromPoint.Y - control.Height / 2);
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
    }
}
