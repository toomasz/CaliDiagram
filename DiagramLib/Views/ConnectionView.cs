using System.Windows.Controls;
using System.Windows.Input;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagramLib.Views
{
    public class ConnectionView : Shape
    {
        public ConnectionView()
        {
            Loaded += ConnectionView_Loaded;
            MouseLeftButtonDown += ConnectionView_MouseLeftButtonDown;
            Cursor = Cursors.Pen;
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add("Remove");
        }

        void ConnectionView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private ConnectionViewModel vm;
        void ConnectionView_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ConnectionViewModel;
            if (vm != null)
                vm.UpdateConnection();
            
        }


        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(ConnectionView),
           new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(ConnectionView),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(ConnectionView),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(ConnectionView),

           new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }


        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get { return (double)base.GetValue(X1Property); }
            set { base.SetValue(X1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get { return (double)base.GetValue(Y1Property); }
            set { base.SetValue(Y1Property, value); }
        }



        protected override Geometry DefiningGeometry
        {
            get
            {

                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    InternalDrawArrowGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Draws an Arrow
        /// <span class="code-SummaryComment"></summary></span>
        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt1 = new Point(X1, Y1); 
            
            double xDiff = X2 - X1;
            Point pt2 = new Point(X1 + (xDiff/3.0), Y1);
            Point pt3 =new Point(X2 - (xDiff / 3.0), Y2);
            Point pt4 = new Point(X2, this.Y2);

            if (vm != null)
            {
                double fromOffset = 60;

                if (vm.AttachPointFrom.Direction == AttachDirection.Top)
                    pt2 = new Point(vm.AttachPointFrom.Location.X, vm.AttachPointFrom.Location.Y - fromOffset);
                if (vm.AttachPointFrom.Direction == AttachDirection.Right)
                    pt2 = new Point(vm.AttachPointFrom.Location.X + fromOffset, vm.AttachPointFrom.Location.Y);
                if (vm.AttachPointFrom.Direction == AttachDirection.Bottom)
                    pt2 = new Point(vm.AttachPointFrom.Location.X, vm.AttachPointFrom.Location.Y + fromOffset);
                if (vm.AttachPointFrom.Direction == AttachDirection.Left)
                    pt2 = new Point(vm.AttachPointFrom.Location.X - fromOffset, vm.AttachPointFrom.Location.Y);

                double toOffset = 60;
                if (vm.AttachPointTo.Direction == AttachDirection.Top)
                    pt3 = new Point(vm.AttachPointTo.Location.X, vm.AttachPointTo.Location.Y - toOffset);
                if (vm.AttachPointTo.Direction == AttachDirection.Right)
                    pt3 = new Point(vm.AttachPointTo.Location.X + toOffset, vm.AttachPointTo.Location.Y);
                if (vm.AttachPointTo.Direction == AttachDirection.Bottom)
                    pt3 = new Point(vm.AttachPointTo.Location.X, vm.AttachPointTo.Location.Y + toOffset);
                if (vm.AttachPointTo.Direction == AttachDirection.Left)
                    pt3 = new Point(vm.AttachPointTo.Location.X - toOffset, vm.AttachPointTo.Location.Y);
            }

            context.BeginFigure(pt1, false, false);

       
            context.BezierTo(pt3, pt2,  pt4, true, true);


        }
    }
}
