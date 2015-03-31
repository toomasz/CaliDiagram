using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork.TaskScheduling
{
    public class TaskSchedlulerTask
    {
        public DateTime SchedluledTime { get; set; }
        public TimeSpan ExecuteAfter { get; set; }
        public Action Function { get; set; }
        /// <summary>
        /// Get milliseconds from DateTime.Now to task execution
        /// </summary>
        public DateTime RunTime
        {
            get
            {
                return SchedluledTime + ExecuteAfter;
            }
        }
        public int RunIn
        {
            get
            {
                int runIn = Convert.ToInt32((RunTime - DateTime.Now).TotalMilliseconds);
                if (runIn < 0)
                    runIn = 0;
                return runIn;
            }
        }
    }
}
