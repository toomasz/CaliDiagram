using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiagramLib.ViewModels;

namespace DiagramLib.Views
{
    /// <summary>
    /// Interaction logic for EdgeView.xaml
    /// </summary>
    public partial class EdgeView : Shape
    {
        public EdgeView()
        {
            Loaded += ConnectionView_Loaded;
           // MouseLeftButtonDown += ConnectionView_MouseLeftButtonDown;
        //    ContextMenu = new ContextMenu();
       //     ContextMenu.Items.Add("Remo");
        }

        void ConnectionView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        //    Caliburn.Micro.Message.SetAttach();
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
            DependencyProperty.Register("FromPoint", typeof(Point), typeof(EdgeView),
             new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));


        public Point ToPoint
        {
            get { return (Point)GetValue(ToPointProperty); }
            set { SetValue(ToPointProperty, value); }
        }

        public static readonly DependencyProperty ToPointProperty =
            DependencyProperty.Register("ToPoint", typeof(Point), typeof(EdgeView),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

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
            vm = DataContext as ConnectionViewModel;
            if (vm == null)
                return;

            if(vm.Type == EdgeLineType.Line)
            {
                context.BeginFigure(FromPoint, false, false);

                context.LineTo(ToPoint, true, false);
            }
            else if (vm.Type == EdgeLineType.Bezier)
            {
                double weirdDiff = ToPoint.Y - FromPoint.X;

                Point pt2 = new Point(FromPoint.X + (weirdDiff / 3.0), FromPoint.Y);
                Point pt3 = new Point(ToPoint.X - (weirdDiff / 3.0), ToPoint.Y);




                double xDiff = Math.Abs(FromPoint.X - ToPoint.X);
                double yDiff = Math.Abs(FromPoint.Y - ToPoint.Y);


                double dist = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

                double xOffset = dist / 2;
                double yOffset = dist / 2;
                if (vm.AttachPointFrom.Direction == AttachDirection.Top)
                    pt2 = new Point(vm.AttachPointFrom.Location.X, vm.AttachPointFrom.Location.Y - yOffset);
                if (vm.AttachPointFrom.Direction == AttachDirection.Right)
                    pt2 = new Point(vm.AttachPointFrom.Location.X + xOffset, vm.AttachPointFrom.Location.Y);
                if (vm.AttachPointFrom.Direction == AttachDirection.Bottom)
                    pt2 = new Point(vm.AttachPointFrom.Location.X, vm.AttachPointFrom.Location.Y + yOffset);
                if (vm.AttachPointFrom.Direction == AttachDirection.Left)
                    pt2 = new Point(vm.AttachPointFrom.Location.X - xOffset, vm.AttachPointFrom.Location.Y);


                if (vm.AttachPointTo.Direction == AttachDirection.Top)
                    pt3 = new Point(vm.AttachPointTo.Location.X, vm.AttachPointTo.Location.Y - yOffset);
                if (vm.AttachPointTo.Direction == AttachDirection.Right)
                    pt3 = new Point(vm.AttachPointTo.Location.X + xOffset, vm.AttachPointTo.Location.Y);
                if (vm.AttachPointTo.Direction == AttachDirection.Bottom)
                    pt3 = new Point(vm.AttachPointTo.Location.X, vm.AttachPointTo.Location.Y + yOffset);
                if (vm.AttachPointTo.Direction == AttachDirection.Left)
                    pt3 = new Point(vm.AttachPointTo.Location.X - xOffset, vm.AttachPointTo.Location.Y);


                context.BeginFigure(FromPoint, false, false);
                context.BezierTo(pt2, pt3, ToPoint, true, true);
            }
        }
    }
}
