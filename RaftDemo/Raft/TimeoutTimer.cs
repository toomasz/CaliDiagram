using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace RaftDemo.Raft
{
    public class TimeoutTimer: IDisposable
    {
        NetworkSoftwareBase software;
        public TimeoutTimer(NetworkSoftwareBase software)
        {
            this.software = software;
            isDisposed = false;
            timer = new Timer();
        }
        Timer timer;

        /// <summary>
        /// Timer not started - actiong will be executed on T+ms
        /// Timer started - stop previous timer - actiong will be executed on T+ms
        /// </summary>
        void SetTimeout(int ms)
        {
            timer.Stop();
            timer.Interval = ms;
            timer.Elapsed -= timer_Elapsed;
            timer.Elapsed += timer_Elapsed;
            
            timer.Start();
            if (Application.Current == null)
                return;
            // update gui
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Application.Current == null)
                    return;
                if (TimerSet != null)
                    TimerSet(this, ms);
            }));

        }
        static Random rnd = new Random();

        /// <summary>
        /// Sets time to timeout in random time between T+msFrom and T+msTo
        /// </summary>
        /// <param name="msFrom">Milliseconds from</param>
        /// <param name="msTo">Milliseconds to</param>
        public void SetRandomTimeout(int msFrom, int msTo)
        {
            double factor = 1;
            msFrom = Convert.ToInt32( factor * msFrom);
            msTo = Convert.ToInt32( factor * msTo);

            if (msFrom > msTo)
                throw new ArgumentException("Invalid timer range");
            int randomPart = rnd.Next(msTo - msFrom);
            int timeout = msFrom + randomPart;
            SetTimeout(timeout);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Application.Current == null)
                return;
            if (isDisposed)
                return;
            timer.Stop();
            software.InputQueue.Add(this);
            if (Elapsed != null)
                Elapsed(this, EventArgs.Empty);
        }
        public event EventHandler<int> TimerSet;
        public event EventHandler<EventArgs> Elapsed;


        bool isDisposed = false;
        public void Dispose()
        {
            isDisposed = true;
            timer.Dispose();
        }
    }
}
