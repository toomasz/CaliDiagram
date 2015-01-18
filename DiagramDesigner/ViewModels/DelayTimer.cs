using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace DiagramDesigner.ViewModels
{
    public class DelayTimer
    {
        Action action;
        public DelayTimer(Action action)
        {
            this.action = action;
        }
        Timer timer = new Timer();
        /// <summary>
        /// Timer not started - actiong will be executed on T+ms
        /// Timer started - stop previous timer - actiong will be executed on T+ms
        /// </summary>
        public void SetElapsed(int ms)
        {
            timer.Interval = ms;
            timer.Elapsed -= timer_Elapsed;
            timer.Elapsed += timer_Elapsed;
            timer.Stop();
            timer.Start();
            if (Application.Current == null)
                return;
            // update gui
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (TimerSet != null)
                    TimerSet(this, ms);
            });

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            action();
            if (Elapsed != null)
                Elapsed(this, EventArgs.Empty);
        }
        public event EventHandler<int> TimerSet;
        public event EventHandler<EventArgs> Elapsed;
    }
}
