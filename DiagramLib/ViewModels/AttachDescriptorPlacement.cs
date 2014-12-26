using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramLib.ViewModels
{
    public enum AttachDirection
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
        Center,
        Float
    };

    public delegate void AttachPointLocationChangedDelegate(AttachPoint ap, Point location);

    public delegate void AttachPointDirectionChangedDelegate(AttachPoint ap, AttachDirection direction);

    public class AttachPoint
    {
        public AttachPoint(DiagramBaseViewModel control, AttachDirection direction)
        {
            Direction = direction;
            Control = control;
        }

        private AttachDirection _direction;
        public AttachDirection Direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    if (DirectionChanging != null)
                        DirectionChanging(this, value);

                    _direction = value;

                }
            }
        }
        public int Order { get; set; }

        public DiagramBaseViewModel Control { get; private set; }

        public event AttachPointLocationChangedDelegate LocationChanged;
        public event AttachPointDirectionChangedDelegate DirectionChanging;

        private Point _Location;
        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    if (LocationChanged != null)
                        LocationChanged(this, Location);
                }
            }
        }

        public Point ControlLocation { get; set; }
    }

    public class AttachDescriptorPlacement
    {
        private DiagramBaseViewModel parent;
        public AttachDescriptorPlacement(DiagramBaseViewModel parentControl)
        {
            parent = parentControl;
            AttachPoints = new Dictionary<AttachDirection, List<AttachPoint>>();
            foreach (AttachDirection type in Enum.GetValues(typeof(AttachDirection)))
                AttachPoints.Add(type, new List<AttachPoint>());

        }

        Point GetPoint(AttachDirection dir, Rect rec)
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

        public Dictionary<AttachDirection, List<AttachPoint>> AttachPoints;

        /// <summary>
        /// Updates attach points belongig to this descriptor(which belongs to entityviewmodel)
        /// When call it?
        /// -Parent control size or location changed
        /// -Any attachment view size or location changed
        /// -On create(When parent view has size, when all attachnment have size
        /// </summary>

        public void UpdatePoints()
        {
            if (AttachPoints.All((kv) => kv.Value.Count == 0))
            {
                return;
            }


            if (parent.Width == 0)
            {
                return;
            }

            if (AttachPoints.Any(kv => kv.Value.Count(a => a.Control.Width == 0) > 0))
            {
                return;
            }


            foreach (var direction in AttachPoints)
            {


                List<Tuple<AttachPoint, DiagramBaseViewModel>> apByPositionOfConnectedTo =
                    new List<Tuple<AttachPoint, DiagramBaseViewModel>>();
                foreach (var ap in direction.Value)
                {
                    ConnectionViewModel cvm = ap.Control.BelongsTo as ConnectionViewModel;
                    if (cvm != null)
                    {
                        DiagramBaseViewModel connTo = (cvm.AttachPointFrom == ap) ? cvm.To : cvm.From;
                        apByPositionOfConnectedTo.Add(Tuple.Create(ap, connTo));
                    }

                }
                int n = 0;

                if (direction.Key == AttachDirection.Left || direction.Key == AttachDirection.Right)
                {
                    foreach (var tuple in apByPositionOfConnectedTo.OrderBy(a => a.Item2.Y))
                        tuple.Item1.Order = n++;
                }
                else if(direction.Key == AttachDirection.Top || direction.Key== AttachDirection.Bottom)
                {
                    foreach (var tuple in apByPositionOfConnectedTo.OrderBy(a => a.Item2.X))
                        tuple.Item1.Order = n++;
                }
                    
                
            }




            foreach (var directionPoints in AttachPoints)
            {
                var direction = directionPoints.Key;
                var attachPoints = directionPoints.Value;


                if (attachPoints.Any(ap => ap.Control.Width == 0))
                {

                }

                if (attachPoints.Count == 0)
                    continue;

                int attachSpacing = 2;
                double groupWidth = attachPoints.Sum(p => p.Control.Width) + attachSpacing * (attachPoints.Count - 1);
                double groupHeight = attachPoints.Sum(p => p.Control.Height) + attachSpacing * (attachPoints.Count - 1);

                var middlePoint = GetPoint(direction, parent.Rect);

                double offsetX = middlePoint.X - (groupWidth / 2.0);
                double offsetY = middlePoint.Y - (groupHeight / 2.0);

                foreach (var attachPoint in attachPoints.OrderBy(a=>a.Order))
                {
                    if (direction == AttachDirection.Top || direction == AttachDirection.Bottom)
                    {
                        attachPoint.Location = new Point(offsetX + (attachPoint.Control.Width / 2), middlePoint.Y);
                        offsetX += attachPoint.Control.Width + attachSpacing;
                    }
                    else if (direction == AttachDirection.Left || direction == AttachDirection.Right)
                    {
                        attachPoint.Location = new Point(middlePoint.X, offsetY + (attachPoint.Control.Height / 2));
                        offsetY += attachPoint.Control.Height + attachSpacing;
                    }

                    attachPoint.ControlLocation = DiagramHelpers.GetAttachmentLocation(attachPoint.Control,
                        attachPoint.Location, attachPoint.Direction);

              
                    attachPoint.Control.SetLocation(attachPoint.ControlLocation.X, attachPoint.ControlLocation.Y);
                  

                }
            }
        }

        public AttachPoint Attach(DiagramBaseViewModel controlToAttach, AttachDirection direction)
        {
            AttachPoint attachPoint = new AttachPoint(controlToAttach, direction);
            attachPoint.Control.SizeChanged += Control_SizeChanged;
            attachPoint.Control.BindingComplete += Control_BindingComplete;
            attachPoint.DirectionChanging += attachPoint_DirectionChanging;
            AttachPoints[direction].Add(attachPoint);
            //UpdatePoints();
            return attachPoint;
        }

        void attachPoint_DirectionChanging(AttachPoint ap, AttachDirection direction)
        {
            AttachPoints[ap.Direction].Remove(ap);
            AttachPoints[direction].Add(ap);
            //UpdatePoints();
        }

        void Control_BindingComplete(object sender, EventArgs e)
        {
            //UpdatePoints();
        }

        void Control_SizeChanged(object sender, EventArgs e)
        {
            //  UpdatePoints();
        }

        public void Detach(AttachPoint point)
        {
            AttachPoints[point.Direction].Remove(point);
            point.Control.BindingComplete -= Control_BindingComplete;
            point.Control.SizeChanged -= Control_SizeChanged;
            point.DirectionChanging -= attachPoint_DirectionChanging;
            //UpdatePoints();
        }

    }
}
