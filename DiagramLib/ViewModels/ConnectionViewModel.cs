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
            Latency = 700;
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

        public PathGeometry PathGeometry1
        {
            get
            {
                PathGeometry animationPath = new PathGeometry();
                PathFigure pFigure = new PathFigure();
                pFigure.StartPoint = AttachPointFrom.Location;

                LineSegment seg = new LineSegment(AttachPointTo.Location, true);
        
               
                pFigure.Segments.Add(seg);
                animationPath.Figures.Add(pFigure);

                // Freeze the PathGeometry for performance benefits.
             //   animationPath.Freeze();
            
                return animationPath;

            }
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
            NotifyOfPropertyChange(() => PathGeometry1);
        }

        private void From_LocationChanged(object sender, EventArgs e)
        {
            UpdateConnection();
            NotifyOfPropertyChange(() => PathGeometry1);
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

        public int Latency
        {
            get;
            set;
        }

        public void SendMessageFrom(NodeBaseViewModel node, object message)
        {
            if (View == null)
                return;
            
            View.SendPacket(node, message);
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
