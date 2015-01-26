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
using System.Windows.Media.Animation;

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
        }

        private ConnectionViewModel vm;
        void ConnectionView_Loaded(object sender, RoutedEventArgs e)
        {
           vm = DataContext as ConnectionViewModel;
           if (vm != null)
               vm.UpdateConnection();

        }


        UIElement CreateVisualForPacket(object packet)
        {
            Rectangle aRectangle = new Rectangle();
            aRectangle.Width = 10;
            aRectangle.Height = 10;
            aRectangle.Fill = Brushes.Red;
            aRectangle.Stroke = Brushes.Black;
            aRectangle.StrokeThickness = 2;
            return aRectangle;
        }

        public void SendPacket(NodeBaseViewModel from, object message)
        {
            if (Dispatcher.CheckAccess())
            {
                SendPacketInternal(from, message);
            }
            else
                Dispatcher.Invoke(() => SendPacketInternal(from, message));
        }
        public void SendPacketInternal(NodeBaseViewModel from, object message)
        {
            UIElement vis = CreateVisualForPacket(message);
            if (vis == null)
                return;

            Canvas canvas = parentCanvas(this);
            if (canvas != null && vm != null)
            {
                PacketView packet = new PacketView(vis, vm, from, canvas);

                packet.Send();
            }
        }

        Canvas parentCanvas(DependencyObject parent)
        {
            Canvas canvas = parent as Canvas;
            if(canvas == null || canvas.Name != "diagram")
            {
                UIElement element = parent as UIElement;
                if(element == null)
                    return null;

                return parentCanvas(VisualTreeHelper.GetParent(parent));
            }
            return canvas;
        }
        List<Rectangle> packetViews = new List<Rectangle>();
 
        public void SendPacketInternal_Old(NodeBaseViewModel from, object message)
        {

            NameScope.SetNameScope(this, new NameScope());

            // Create a rectangle.
            Rectangle aRectangle = new Rectangle();
            aRectangle.Width = 10;
            aRectangle.Height = 10;
            aRectangle.Fill = Brushes.Red;
            aRectangle.Stroke = Brushes.Black;
            aRectangle.StrokeThickness = 2;
            packetViews.Add(aRectangle);

            Canvas.SetZIndex(aRectangle, 0);
           
            // Create a transform. This transform 
            // will be used to move the rectangle.
            TranslateTransform animatedTranslateTransform =
                new TranslateTransform();
            Canvas canvas = parentCanvas(this);
            canvas.Children.Add(aRectangle);


            // Register the transform's name with the page 
            // so that they it be targeted by a Storyboard. 
            this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);

            aRectangle.RenderTransform = animatedTranslateTransform;


            // Create the animation path.
            PathGeometry animationPath = new PathGeometry();

            PathFigure pFigure = new PathFigure();
            var points = GetBezierPoints();
            
            PolyBezierSegment pBezierSegment = new PolyBezierSegment();

            if (from == vm.From)
            {
                pFigure.StartPoint = points[0];
                pBezierSegment.Points.Add(points[1]);
                pBezierSegment.Points.Add(points[2]);
                pBezierSegment.Points.Add(points[3]);

                pFigure.Segments.Add(pBezierSegment);
            }
            else if (from == vm.To)
            {
                pFigure.StartPoint = points[3];
                pBezierSegment.Points.Add(points[2]);
                pBezierSegment.Points.Add(points[1]);
                pBezierSegment.Points.Add(points[0]);

                pFigure.Segments.Add(pBezierSegment);
            }
            animationPath.Figures.Add(pFigure);

            // Freeze the PathGeometry for performance benefits.
            animationPath.Freeze();

            // Create a DoubleAnimationUsingPath to move the 
            // rectangle horizontally along the path by animating  
            // its TranslateTransform.
            DoubleAnimationUsingPath translateXAnimation =
                new DoubleAnimationUsingPath();
            translateXAnimation.PathGeometry = animationPath;
            translateXAnimation.Duration = TimeSpan.FromMilliseconds(vm.Latency);

            // Set the Source property to X. This makes 
            // the animation generate horizontal offset values from 
            // the path information. 
            translateXAnimation.Source = PathAnimationSource.X;
            translateXAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromMilliseconds(vm.Latency));
            // Set the animation to target the X property 
            // of the TranslateTransform named "AnimatedTranslateTransform".
            Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translateXAnimation,
                new PropertyPath(TranslateTransform.XProperty));

            // Create a DoubleAnimationUsingPath to move the 
            // rectangle vertically along the path by animating  
            // its TranslateTransform.
            DoubleAnimationUsingPath translateYAnimation =
                new DoubleAnimationUsingPath();
            translateYAnimation.PathGeometry = animationPath;
            translateYAnimation.Duration = TimeSpan.FromMilliseconds(vm.Latency);
            translateYAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromMilliseconds(vm.Latency));


            // Set the Source property to Y. This makes 
            // the animation generate vertical offset values from 
            // the path information. 
            translateYAnimation.Source = PathAnimationSource.Y;

            // Set the animation to target the Y property 
            // of the TranslateTransform named "AnimatedTranslateTransform".
            Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translateYAnimation,
                new PropertyPath(TranslateTransform.YProperty));

            // Create a Storyboard to contain and apply the animations.
            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.RepeatBehavior = new RepeatBehavior(1);
            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);
            aRectangle.Tag = pathAnimationStoryboard;
            // Start the animations when the rectangle is loaded.
            aRectangle.Loaded += delegate(object sender, RoutedEventArgs e)
            {
                // Start the storyboard.
                pathAnimationStoryboard.Completed += pathAnimationStoryboard_Completed;
              
                pathAnimationStoryboard.Begin(this);
                
            };
           
        }

        void pathAnimationStoryboard_Completed(object sender, EventArgs e)
        {
            ClockGroup cg = sender as ClockGroup;
            Storyboard sb = cg.Timeline as Storyboard;
            Rectangle rect = packetViews.FirstOrDefault(pv => pv.Tag == sb);
            if(rect != null)
            {
                Canvas canvas = parentCanvas(this);
                canvas.Children.Remove(rect);
                packetViews.Remove(rect);
            }
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


        List<Point> GetBezierPoints()
        {
            double weirdDiff = ToPoint.Y - FromPoint.X;

            Point pt2 = new Point(FromPoint.X + (weirdDiff / 3.0), FromPoint.Y);
            Point pt3 = new Point(ToPoint.X - (weirdDiff / 3.0), ToPoint.Y);




            double xDiff = Math.Abs(FromPoint.X - ToPoint.X);
            double yDiff = Math.Abs(FromPoint.Y - ToPoint.Y);


            double dist = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            double xOffset = dist / 2;
            double yOffset = dist / 2;
            if (vm.AttachPointFrom.Side == AttachDirection.Top)
                pt2 = new Point(vm.AttachPointFrom.Location.X, vm.AttachPointFrom.Location.Y - yOffset);
            if (vm.AttachPointFrom.Side == AttachDirection.Right)
                pt2 = new Point(vm.AttachPointFrom.Location.X + xOffset, vm.AttachPointFrom.Location.Y);
            if (vm.AttachPointFrom.Side == AttachDirection.Bottom)
                pt2 = new Point(vm.AttachPointFrom.Location.X, vm.AttachPointFrom.Location.Y + yOffset);
            if (vm.AttachPointFrom.Side == AttachDirection.Left)
                pt2 = new Point(vm.AttachPointFrom.Location.X - xOffset, vm.AttachPointFrom.Location.Y);


            if (vm.AttachPointTo.Side == AttachDirection.Top)
                pt3 = new Point(vm.AttachPointTo.Location.X, vm.AttachPointTo.Location.Y - yOffset);
            if (vm.AttachPointTo.Side == AttachDirection.Right)
                pt3 = new Point(vm.AttachPointTo.Location.X + xOffset, vm.AttachPointTo.Location.Y);
            if (vm.AttachPointTo.Side == AttachDirection.Bottom)
                pt3 = new Point(vm.AttachPointTo.Location.X, vm.AttachPointTo.Location.Y + yOffset);
            if (vm.AttachPointTo.Side == AttachDirection.Left)
                pt3 = new Point(vm.AttachPointTo.Location.X - xOffset, vm.AttachPointTo.Location.Y);


            return new List<Point>() { FromPoint, pt2, pt3, ToPoint };
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
                var bezierPoints =  GetBezierPoints();

                context.BeginFigure(bezierPoints[0], false, false);
                context.BezierTo(bezierPoints[1], bezierPoints[2], bezierPoints[3], true, true);
            }
        }
    }
}
