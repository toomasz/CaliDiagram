using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public PacketView(UIElement vis, ConnectionViewModel connection, NodeBaseViewModel from, Canvas canvas)
        {
            this.Visual = vis;
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
        public UIElement Visual
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
            Canvas.Children.Add(Visual);
            Visual.RenderTransform = new MatrixTransform(new Matrix());

            Visual.RenderTransformOrigin = new Point(0, 0);
            

            MatrixAnimationUsingPath anim = new MatrixAnimationUsingPath();
            anim.Duration = TimeSpan.FromMilliseconds(Connection.Latency);
            anim.DoesRotateWithTangent = true;
            anim.RepeatBehavior = new RepeatBehavior(1);
            anim.PathGeometry = Connection.PathGeometry1;

            Storyboard.SetTarget(anim, Visual.RenderTransform);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Matrix"));
           
            Storyboard sb = new Storyboard();
            sb.RepeatBehavior = new RepeatBehavior(1);
            sb.Children.Add(anim);

            sb.Begin();
            sb.Completed += sb_Completed;
          
        }

        void sb_Completed(object sender, EventArgs e)
        {
           
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
