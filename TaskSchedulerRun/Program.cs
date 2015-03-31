
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NetworkModel.InProcNetwork;
using System;
using System.Linq;
using System.Threading.Tasks;
using Animatroller.Framework.Utility;

namespace Animatroller.Framework.Utility
{
 
}
namespace TaskSchedulerRun
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSchedulerRun run = new TestSchedulerRun();
            run.Start();
        }
    }
   
    class TestSchedulerRun
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint uMilliseconds);

       

     
        HighPrecisionTimer swTimer = new HighPrecisionTimer(19);
        Stopwatch sw = new Stopwatch();
        volatile int k = 0;
        void swTimer_Tick(object sender, HighPrecisionTimer.TickEventArgs e)
        {
            sw.Stop();
            Console.WriteLine("time: " + sw.ElapsedMilliseconds);

            e.NextTickIn = 1 + k;
            k++;
            sw.Restart();
        }

        public void Start()
        {
            new TsTest().TestSchedluler();

            TimeBeginPeriod(1);
          //  sw.Start();
         //   swTimer.Tick += swTimer_Tick;
           

           // Thread.Sleep(10000);
            Console.ReadKey();
        }

        void HandleAndFireNextTask()
        {
            Console.WriteLine("xxx");
          // ts.SchedluleTask(()=>HandleAndFireNextTask(), TimeSpan.FromMilliseconds(500));
        }

    }
}
