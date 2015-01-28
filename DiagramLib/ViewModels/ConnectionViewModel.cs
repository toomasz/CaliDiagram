using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using DiagramLib.Views;
using System.Windows.Controls;

namespace DiagramLib.ViewModels
{
    /// <summary>
    /// ViewModel representing connection between two resource tree controllers
    /// </summary>

    public enum EdgeLineType { Bezier, Line};

    public class ConnectionViewModel : PropertyChangedBase, IDisposable, IViewAware
    {
        public ConnectionViewModel(NodeBaseViewModel from, NodeBaseViewModel to)
        {
            this.From = from;
            this.To = to;

            From.LocationChanged += From_LocationChanged;
            To.LocationChanged += To_LocationChanged;

            StrokeThickness = 2;
            Stroke = Brushes.DarkOliveGreen;
            Type = EdgeLineType.Bezier;
         
        }

        public DiagramViewModel ParentDiagram { get; internal set; }

        private EdgeLineType _Type;
        public EdgeLineType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    NotifyOfPropertyChange(() => Type);
                }
            }
        }
        
        private double _StrokeThickness;
        public double StrokeThickness
        {
            get { return _StrokeThickness; }
            set
            {
                if (_StrokeThickness != value)
                {
                    _StrokeThickness = value;
                    NotifyOfPropertyChange(() => StrokeThickness);
                }
            }
        }

        public List<Point> GetBezierPoints()
        {
            Point FromPoint = AttachPointFrom.Location;
            Point ToPoint = AttachPointTo.Location;

            double weirdDiff = ToPoint.Y - FromPoint.X;

            Point pt2 = new Point(FromPoint.X + (weirdDiff / 3.0), FromPoint.Y);
            Point pt3 = new Point(ToPoint.X - (weirdDiff / 3.0), ToPoint.Y);




            double xDiff = Math.Abs(FromPoint.X - ToPoint.X);
            double yDiff = Math.Abs(FromPoint.Y - ToPoint.Y);


            double dist = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            double xOffset = dist / 2;
            double yOffset = dist / 2;
            if (AttachPointFrom.Side == AttachDirection.Top)
                pt2 = new Point(AttachPointFrom.Location.X, AttachPointFrom.Location.Y - yOffset);
            if (AttachPointFrom.Side == AttachDirection.Right)
                pt2 = new Point(AttachPointFrom.Location.X + xOffset, AttachPointFrom.Location.Y);
            if (AttachPointFrom.Side == AttachDirection.Bottom)
                pt2 = new Point(AttachPointFrom.Location.X, AttachPointFrom.Location.Y + yOffset);
            if (AttachPointFrom.Side == AttachDirection.Left)
                pt2 = new Point(AttachPointFrom.Location.X - xOffset, AttachPointFrom.Location.Y);


            if (AttachPointTo.Side == AttachDirection.Top)
                pt3 = new Point(AttachPointTo.Location.X, AttachPointTo.Location.Y - yOffset);
            if (AttachPointTo.Side == AttachDirection.Right)
                pt3 = new Point(AttachPointTo.Location.X + xOffset, AttachPointTo.Location.Y);
            if (AttachPointTo.Side == AttachDirection.Bottom)
                pt3 = new Point(AttachPointTo.Location.X, AttachPointTo.Location.Y + yOffset);
            if (AttachPointTo.Side == AttachDirection.Left)
                pt3 = new Point(AttachPointTo.Location.X - xOffset, AttachPointTo.Location.Y);


            return new List<Point>() { FromPoint, pt2, pt3, ToPoint };
        }

        private Brush _Stroke;
        public Brush Stroke 
        {
            get { return _Stroke; }
            set
            {
                if (_Stroke != value)
                {
                    _Stroke = value;
                    NotifyOfPropertyChange(() => Stroke);
                }
            }
        }

        private void To_LocationChanged(object sender, EventArgs e)
        {
            UpdateConnection();
        }

        private void From_LocationChanged(object sender, EventArgs e)
        {
            UpdateConnection();
        }

        public NodeBaseViewModel FromDescriptor { get; set; }
        public NodeBaseViewModel ToDescriptor { get; set; }

        /// <summary>
        /// 0 - top
        /// 1 - right
        /// 2 - bottom
        /// 3 - left
        /// </summary>
        public void UpdateConnection()
        {
           // Console.WriteLine("UpdateConnection {0} -> {1}", From.Name, To.Name);
            if (AttachPointFrom == null || AttachPointTo == null)
                return;
            var newOrSameSides = ParentDiagram.Definition.ConnectorSideStrategy.GetSidesForConnection(this);
  

            AttachPointFrom.Side = newOrSameSides.FromSide;
            AttachPointTo.Side = newOrSameSides.ToSide;

            From.UpdateAttachPoints();
            To.UpdateAttachPoints();           
        }

        public NodeBaseViewModel From
        {
            get;
            set;
        }
        public NodeBaseViewModel To
        {
            get;
            set;
        }

        Random rnd = new Random();
        public int Latency
        {
            get
            {
                return 600 +rnd.Next(400);
            }
        }
        public void SendPacket(NodeBaseViewModel from, object message)
        {
            try
            {
                if (ParentDiagram.Canvas.Dispatcher.CheckAccess())
                {
                    SendPacketInternal(from, message);
                }
                else
                    ParentDiagram.Canvas.Dispatcher.Invoke(() => SendPacketInternal(from, message));
            }
            catch (Exception ex)
            {

            }
        }
        public void SendPacketInternal(NodeBaseViewModel from, object message)
        {
            // packet view is created in definition
            FrameworkElement vis = ParentDiagram.Definition.CreateVisualForPacket(message);
            if (vis == null)
                return;

            Canvas canvas = ParentDiagram.Canvas;
            if (canvas != null)
            {
                PacketView packet = new PacketView(vis, this, from, canvas);

                packet.Send();
            }
        }

        public void StartMessageAnimationFrom(NodeBaseViewModel node, object message)
        {
            SendPacket(node, message);
        }

        private AttachPoint _AttachPointFrom;
        public AttachPoint AttachPointFrom
        {
            get { return _AttachPointFrom; }
            set
            {
                if (_AttachPointFrom != value)
                {
                    _AttachPointFrom = value;
                    NotifyOfPropertyChange(() => AttachPointFrom);
                }
            }
        }

        private AttachPoint _AttachPointTo;
        public AttachPoint AttachPointTo
        {
            get { return _AttachPointTo; }
            set
            {
                if (_AttachPointTo != value)
                {
                    _AttachPointTo = value;
                    NotifyOfPropertyChange(() => AttachPointTo);
                }
            }
        }
        
        public void Dispose()
        {
            From.LocationChanged -= From_LocationChanged;
            To.LocationChanged -= To_LocationChanged;

            From.Detach(AttachPointFrom);
            To.Detach(AttachPointTo);
        }

        EdgeView View;
        public void AttachView(object view, object context = null)
        {
            View = view as EdgeView;
        }

        public object GetView(object context = null)
        {
            return View;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;
    }
}
