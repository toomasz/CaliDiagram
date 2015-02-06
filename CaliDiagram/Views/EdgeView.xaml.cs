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
using CaliDiagram.ViewModels;
using System.Windows.Media.Animation;

namespace CaliDiagram.Views
{
    /// <summary>
    /// Interaction logic for EdgeView.xaml
    /// </summary>
    public partial class EdgeView : Shape
    {
        public EdgeView()
        {
            Loaded += ConnectionView_Loaded;            
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
                var bezierPoints = vm.GetBezierPoints();

                context.BeginFigure(bezierPoints[0], false, false);
                context.BezierTo(bezierPoints[1], bezierPoints[2], bezierPoints[3], true, true);
            }
        }
    }
}
