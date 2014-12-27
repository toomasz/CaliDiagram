using DiagramLib.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;
                
                using (StreamGeometryContext context = geometry.Open())
                    InternalDrawArrowGeometry(context);
                
                geometry.Freeze();

                return geometry;
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Draws an Arrow
        /// <span class="code-SummaryComment"></summary></span>
        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double xDiff = ToPoint.Y - FromPoint.X;

            Point pt2 = new Point(FromPoint.X + (xDiff/3.0), FromPoint.Y);
            Point pt3 =new Point(ToPoint.X - (xDiff / 3.0), ToPoint.Y);
      

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
