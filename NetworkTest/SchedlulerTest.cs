using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkModel.InProcNetwork;
using NetworkModel.InProcNetwork.TaskScheduling;
using NFluent;


namespace NetworkTest
{
    [TestClass]
    public class SchedlulerTest
    {
        class TimingTestTask
        {
            public int Id { get; set; }
            public int ExpectedDelay { get; set; }
            public int ActualDelay { get; set; }
            public bool WasExecuted { get; private set; }

            Stopwatch sw = new Stopwatch();
            public void StartCounting()
            {
                sw.Start();
            }
            public void Executed()
            {
                WasExecuted = true;
                ActualDelay = Convert.ToInt32(sw.ElapsedMilliseconds);
                sw.Stop();
            }
        }
        [TestMethod]
        public void TestSchedluler()
        {
            List<int> taskTimings = new List<int>();
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);
            taskTimings.Add(400);
            taskTimings.Add(54);
            taskTimings.Add(774);

            TaskScheduler ts = new TaskScheduler();

            List<TimingTestTask> testTasks = new List<TimingTestTask>();
            // create tasks
            for (int i = 0; i < taskTimings.Count; i++ )
            {
                TimingTestTask task = new TimingTestTask() { ExpectedDelay = taskTimings[i], Id = i };
                testTasks.Add(task);
                if (i % 11 == 0)
                    Thread.Sleep(1000);
            }

            foreach(var task in testTasks)
            {
                task.StartCounting();
                ts.SchedluleTask(task.Executed, TimeSpan.FromMilliseconds(task.ExpectedDelay));
            }


            Thread.Sleep(5000);
            
            decimal meanAbsoluteError = 0;
            foreach(var task in testTasks)
            {
                Check.That(task.WasExecuted);
                meanAbsoluteError += Math.Abs(task.ActualDelay - task.ExpectedDelay);
            }
            meanAbsoluteError /= testTasks.Count;
            Check.That(meanAbsoluteError).IsLessThan(10);
        }
    }
}
