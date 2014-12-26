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
        public ConnectionViewModel(DiagramBaseViewModel from, DiagramBaseViewModel to, DiagramBaseViewModel fromAd, DiagramBaseViewModel toAd,
            DiagramViewModel diagramVm
            )
        {
            this.Diagram = diagramVm;
            this.FromDescriptor = fromAd;
            this.ToDescriptor = toAd;
            FromDescriptor.BelongsTo = this;
            ToDescriptor.BelongsTo = this;

            this.From = from;
            this.To = to;

            UpdateConnection();

            AttachPointFrom.LocationChanged += AttachPointFrom_LocationChanged;
            AttachPointTo.LocationChanged += AttachPointToOnLocationChanged;
            From.LocationChanged += From_LocationChanged;
            To.LocationChanged += To_LocationChanged;

            //    From.SizeChanged += From_SizeChanged;
            //     To.SizeChanged += To_SizeChanged;
            From.BindingComplete += From_BindingComplete;
            To.BindingComplete += To_BindingComplete;
            // FromDescriptor = new AttachDescriptorVm();
            //   ToDescriptor = new AttachDescriptorVm();

        }

        private DiagramViewModel Diagram;

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

        private void AttachPointToOnLocationChanged(AttachPoint ap, Point location)
        {
            X1 = location.X;
            Y1 = location.Y;
        }

        void AttachPointFrom_LocationChanged(AttachPoint ap, Point location)
        {
            X2 = location.X;
            Y2 = location.Y;
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



        /// <summary>
        /// 0 - top
        /// 1 - right
        /// 2 - bottom
        /// 3 - left
        /// </summary>
        public void UpdateConnection()
        {
            var attachPointsFrom = AttachPoints(From.Rect);
            var attachPointsTo = AttachPoints(To.Rect);

            var results = new List<Tuple<Point, Point, double, int, int>>();
            for (int i = 0; i < attachPointsFrom.Length; i++)
            {
                for (int j = 0; j < attachPointsTo.Length; j++)
                    results.Add(Tuple.Create(attachPointsFrom[i], attachPointsTo[j], DistanceBetweenPoints(attachPointsFrom[i], attachPointsTo[j]), i, j));
            }

            var bestMatch = results.OrderBy(r => r.Item3).First();


            if (AttachPointFrom == null)
                AttachPointFrom = From.Attach(FromDescriptor, (AttachDirection) bestMatch.Item4);
            else
                AttachPointFrom.Direction = (AttachDirection)bestMatch.Item4;

            if (AttachPointTo == null)
                AttachPointTo = To.Attach(ToDescriptor, (AttachDirection)bestMatch.Item5);
            else
                AttachPointTo.Direction = (AttachDirection)bestMatch.Item5;

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

        private double _X1;
        public double X1
        {
            get { return _X1; }
            set
            {
                if (_X1 != value)
                {
                    _X1 = value;
                    NotifyOfPropertyChange(() => X1);
                }
            }
        }
        private double _Y1;
        public double Y1
        {
            get { return _Y1; }
            set
            {
                if (_Y1 != value)
                {
                    _Y1 = value;
                    NotifyOfPropertyChange(() => Y1);
                }
            }
        }

        private double _X2;
        public double X2
        {
            get { return _X2; }
            set
            {
                if (_X2 != value)
                {
                    _X2 = value;
                    NotifyOfPropertyChange(() => X2);
                }
            }
        }

        private double _Y2;
        public double Y2
        {
            get { return _Y2; }
            set
            {
                if (_Y2 != value)
                {
                    _Y2 = value;
                    NotifyOfPropertyChange(() => Y2);
                }
            }
        }

        public AttachPoint AttachPointFrom;
        public AttachPoint AttachPointTo;

        public void Dispose()
        {

            From.LocationChanged -= From_LocationChanged;
            To.LocationChanged -= To_LocationChanged;
        }
    }
}
