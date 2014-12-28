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

namespace DiagramLib.ViewModels
{
    /// <summary>
    /// ViewModel representing connection between two resource tree controllers
    /// </summary>
    public class ConnectionViewModel : PropertyChangedBase, IDisposable
    {
        public ConnectionViewModel(DiagramBaseViewModel from, DiagramBaseViewModel to)
        {
            this.From = from;
            this.To = to;

            From.LocationChanged += From_LocationChanged;
            To.LocationChanged += To_LocationChanged;

            From.BindingComplete += From_BindingComplete;
            To.BindingComplete += To_BindingComplete;
            StrokeThickness = 2;
            Stroke = Brushes.DarkOliveGreen;
            
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
        

        void To_BindingComplete(object sender, EventArgs e)
        {
            UpdateConnection();
        }

        void From_BindingComplete(object sender, EventArgs e)
        {
            UpdateConnection();
        }

        private void To_LocationChanged(object sender, EventArgs e)
        {
            UpdateConnection();
        }

        private void From_LocationChanged(object sender, EventArgs e)
        {
            UpdateConnection();
        }

        public DiagramBaseViewModel FromDescriptor { get; set; }
        public DiagramBaseViewModel ToDescriptor { get; set; }

        Point[] AttachPoints(Rect rect)
        {
            return new Point[]
            {
                DiagramHelpers.GetMiddlePoint(AttachDirection.Top, rect),
                DiagramHelpers.GetMiddlePoint(AttachDirection.Right, rect),
                DiagramHelpers.GetMiddlePoint(AttachDirection.Bottom, rect),
                DiagramHelpers.GetMiddlePoint(AttachDirection.Left, rect)
            };

        }

        double DistanceBetweenPoints(Point a, Point b)
        {
            double d = (Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            return d;
        }

        Tuple<AttachDirection, AttachDirection> GetAttachDirections(Rect fromRect, Rect toRect)
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
            return Tuple.Create((AttachDirection) bestMatch.Item4, (AttachDirection) bestMatch.Item5);
        }

        /// <summary>
        /// 0 - top
        /// 1 - right
        /// 2 - bottom
        /// 3 - left
        /// </summary>
        public void UpdateConnection()
        {
            var bestDirections = GetAttachDirections(From.Rect, To.Rect);
            
            if (AttachPointFrom == null)
            {
                AttachPointFrom = From.Attach(bestDirections.Item1, this);
                AttachPointFrom.Control = FromDescriptor;
            }
            else
                AttachPointFrom.Direction = bestDirections.Item1;

            if (AttachPointTo == null)
            {
                AttachPointTo = To.Attach(bestDirections.Item2, this);
                AttachPointTo.Control = ToDescriptor;
            }
            else
                AttachPointTo.Direction = bestDirections.Item2;

            To.UpdateAttachPoints();
            From.UpdateAttachPoints();
        }

        public DiagramBaseViewModel From
        {
            get;
            set;
        }
        public DiagramBaseViewModel To
        {
            get;
            set;
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
            From.BindingComplete -= From_BindingComplete;
            To.BindingComplete -= To_BindingComplete;
            From.Detach(AttachPointFrom);
            To.Detach(AttachPointTo);
        }
    }
}
