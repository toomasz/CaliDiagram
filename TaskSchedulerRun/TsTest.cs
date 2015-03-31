using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NetworkModel.InProcNetwork.TaskScheduling;

namespace TaskSchedulerRun
{
    class TsTest
    {
        class TimingTestTask
        {
            public int Id { get; set; }
            public int ExpectedDelay { get; set; }
            public int ActualDelay { get; set; }
            public volatile int WasExecuted = 0;

            Stopwatch sw = new Stopwatch();
            public void StartCounting()
            {
                sw.Start();
            }
            public void Executed()
            {
                Interlocked.Increment(ref WasExecuted);
                ActualDelay = Convert.ToInt32(sw.ElapsedMilliseconds);
                sw.Stop();
            }
        }
       
        public void TestSchedluler()
        {
            List<int> taskTimings = new List<int>();
            

            for(int i=0; i < 1000; i++)
            {
                taskTimings.Add(i + 5);
                taskTimings.Add(i /2 + 100);
            }

            TaskScheduler ts = new TaskScheduler();

            List<TimingTestTask> testTasks = new List<TimingTestTask>();
            // create tasks
            for (int i = 0; i < taskTimings.Count; i++)
            {
                TimingTestTask task = new TimingTestTask() { ExpectedDelay = taskTimings[i], Id = i };
                testTasks.Add(task);
                
            }
            int executedTasks = 0;

            foreach (var task in testTasks)
            {
                task.StartCounting();
                ts.SchedluleTask(() => { task.Executed(); Interlocked.Increment(ref executedTasks); }, TimeSpan.FromMilliseconds(task.ExpectedDelay));
            }


            Thread.Sleep(11000);
            Thread.MemoryBarrier();
            decimal meanAbsoluteError = 0;
            foreach (var task in testTasks)
            {
                if (task.WasExecuted != 1)
                    Console.WriteLine("Task {0} was not executed !", task.Id);

                meanAbsoluteError += Math.Abs(task.ActualDelay - task.ExpectedDelay);

                Console.WriteLine("Exptected: {0}\tActual: {1}", task.ExpectedDelay, task.ActualDelay);
            }
            meanAbsoluteError /= testTasks.Count;
            Console.WriteLine("Total: {0}\tExecuted: {1}", testTasks.Count, executedTasks);
            Console.WriteLine("MAE: {0}", meanAbsoluteError);
        }
    }
}
