using System.Timers;
using System.Windows.Media;
using System.Windows.Threading;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DiagramDesigner.ViewModels
{
    class ThickConnectionViewModel: ConnectionViewModel
    {
        public ThickConnectionViewModel(DiagramBaseViewModel from, DiagramBaseViewModel to) :
            base(from, to)
        {
            StrokeThickness = 7;
            Stroke = Brushes.Red;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private int n = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            n++;
            if (n%2 == 0)
            {
                Stroke = Brushes.Red;
            }
            else
            {

                Stroke = Brushes.Orange;
            }
        }
    }
}
