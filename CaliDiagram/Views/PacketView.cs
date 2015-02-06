using CaliDiagram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CaliDiagram.Views
{
    public class PacketView
    {

        public PacketView(FrameworkElement view, ConnectionViewModel connection, NodeBaseViewModel from, Canvas canvas)
        {
            this.View = view;
            this.Canvas = canvas;
            this.From = from;
            this.Connection = connection;
        }
        public ConnectionViewModel Connection
        {
            get;
            private set;
        }
        public NodeBaseViewModel From
        {
            get;
            private set;
        }
        public FrameworkElement View
        {
            get;
            private set;
        }
        public Canvas Canvas
        {
            get;
            private set;
        }

        // this is always called from ui thread
        public void Send()
        {
            Canvas.Children.Add(View);
            View.Loaded += View_Loaded;
            View.Visibility = Visibility.Hidden;
            View.InvalidateMeasure();
            View.InvalidateVisual();

            NameScope.SetNameScope(Canvas, new NameScope());

            MatrixTransform buttonMatrixTransform = new MatrixTransform();         
            View.RenderTransform = buttonMatrixTransform;           

            string transformName = "ButtonMatrixTransform";

            Canvas.RegisterName(transformName, buttonMatrixTransform);

            MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath()
            {
                PathGeometry = GetAnimationPathGeometry(new Point(-View.Width/2,-View.Height/2)),
                DoesRotateWithTangent = false,
                IsOffsetCumulative = true,
                IsAdditive = true,
                Duration = TimeSpan.FromMilliseconds(Connection.Latency),
                RepeatBehavior = new RepeatBehavior(1)
            };
            
            Storyboard.SetTargetName(matrixAnimation, transformName);         
            Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));

            Storyboard packetAnimationStoryboard = new Storyboard();
            
            packetAnimationStoryboard.Children.Add(matrixAnimation);

        
            packetAnimationStoryboard.Completed += pathAnimationStoryboard_Completed;
            packetAnimationStoryboard.Begin(Canvas);

            packetAnimationStoryboard.Freeze();
            matrixAnimation.Freeze();

            View.Visibility = Visibility.Visible;
        }

        void View_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        PathGeometry GetAnimationPathGeometry(Point offset)
        {
            PathGeometry animationPath = new PathGeometry();

            PathFigure pFigure = new PathFigure();
            var points = Connection.GetBezierPoints();

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                p.Offset(offset.X, offset.Y);
                points[i] = p;
            }

            PolyBezierSegment pBezierSegment = new PolyBezierSegment();

            if (From == Connection.From)
            {
                pFigure.StartPoint = points[0];
                pBezierSegment.Points.Add(points[1]);
                pBezierSegment.Points.Add(points[2]);
                pBezierSegment.Points.Add(points[3]);
            }
            else if (From == Connection.To)
            {
                pFigure.StartPoint = points[3];
                pBezierSegment.Points.Add(points[2]);
                pBezierSegment.Points.Add(points[1]);
                pBezierSegment.Points.Add(points[0]);                
            }
            foreach (Point p in pBezierSegment.Points)
                p.Offset(4, 4);

            pFigure.Segments.Add(pBezierSegment);
            animationPath.Figures.Add(pFigure);

            // Freeze the PathGeometry for performance benefits.
            animationPath.Freeze();
            
            return animationPath;
        }

        void pathAnimationStoryboard_Completed(object sender, EventArgs e)
        {
            Canvas.Children.Remove(View);            
        }

     
        public void Pause()
        {

        }
        public void Unpause()
        {

        }

        void RemoveMyself()
        {

        }
    }
}
