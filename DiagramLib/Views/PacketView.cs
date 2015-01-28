using DiagramLib.ViewModels;
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

namespace DiagramLib.Views
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
        static int i = 0;
        // this is always called from ui thread
        public void Send()
        {

            i++;
          //  Canvas.SetLeft(Visual, -100);
           // Canvas.SetTop(Visual, -100);

            Canvas.Children.Add(View);

            View.Visibility = Visibility.Hidden;
           // Connection.ParentDiagram.ForceRedraw();
            NameScope.SetNameScope(Canvas, new NameScope());

            MatrixTransform buttonMatrixTransform = new MatrixTransform();
            View.RenderTransform = buttonMatrixTransform;
            buttonMatrixTransform.Matrix = new Matrix();
            buttonMatrixTransform.Matrix.Translate(-100, -100);

            string transformName = string.Format("ButtonMatrixTransform");


            Canvas.RegisterName(transformName, buttonMatrixTransform);

            MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
            matrixAnimation.PathGeometry = GetAnimationPathGeometry();
            matrixAnimation.DoesRotateWithTangent = false;

  
            matrixAnimation.IsOffsetCumulative = true;
            matrixAnimation.Duration = TimeSpan.FromMilliseconds(Connection.Latency);
            matrixAnimation.RepeatBehavior = new RepeatBehavior(1);
        

            // Set the animation to target the Matrix property
            // of the MatrixTransform named "ButtonMatrixTransform".
            Storyboard.SetTargetName(matrixAnimation, transformName);
         
            Storyboard.SetTargetProperty(matrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));


            // Create a Storyboard to contain and apply the animation.
            Storyboard pathAnimationStoryboard = new Storyboard();
            
            pathAnimationStoryboard.Children.Add(matrixAnimation);

        
            pathAnimationStoryboard.Completed += pathAnimationStoryboard_Completed;
            pathAnimationStoryboard.Begin(Canvas);

            pathAnimationStoryboard.Freeze();
            matrixAnimation.Freeze();

            View.Visibility = Visibility.Visible;
        }

        PathGeometry GetAnimationPathGeometry()
        {
            PathGeometry animationPath = new PathGeometry();

            PathFigure pFigure = new PathFigure();
            var points = Connection.GetBezierPoints();

            PolyBezierSegment pBezierSegment = new PolyBezierSegment();

            if (From == Connection.From)
            {
                pFigure.StartPoint = points[0];
                pBezierSegment.Points.Add(points[1]);
                pBezierSegment.Points.Add(points[2]);
                pBezierSegment.Points.Add(points[3]);

                pFigure.Segments.Add(pBezierSegment);
            }
            else if (From == Connection.To)
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
