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


        public Point FromPoint
        {
            get { return (Point)GetValue(FromPointProperty); }
            set { SetValue(FromPointProperty, value); }
        }

        public static readonly DependencyProperty FromPointProperty =
            DependencyProperty.Register("FromPoint", typeof(Point), typeof(ConnectionView),
             new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));


        public Point ToPoint
        {
            get { return (Point)GetValue(ToPointProperty); }
            set { SetValue(ToPointProperty, value); }
        }

        public static readonly DependencyProperty ToPointProperty =
            DependencyProperty.Register("ToPoint", typeof(Point), typeof(ConnectionView), 
            new FrameworkPropertyMetadata(new Point(0,0), FrameworkPropertyMetadataOptions.AffectsRender));

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

   
            
            double xDiff = X2 - X1;
            Point pt2 = new Point(X1 + (xDiff/3.0), Y1);
            Point pt3 =new Point(X2 - (xDiff / 3.0), Y2);
      

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

            context.BeginFigure(FromPoint, false, false);

       
            context.BezierTo(pt3, pt2,  ToPoint, true, true);
        }
    }
}
