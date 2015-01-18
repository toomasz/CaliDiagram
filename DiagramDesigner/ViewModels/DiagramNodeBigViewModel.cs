using System.Collections.ObjectModel;
using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DiagramDesigner.ViewModels
{
    class Message
    {
        public int Clock { get; set; }
        public string Value { get; set; }
    }
    class DiagramNodeBigViewModel : NetworkNodeViewModel
    {
        public DiagramNodeBigViewModel(string name)
        {
            this.Name = name;
            RaftTimer = new DelayTimer(RaftTimerElapsed);
        }
        public DelayTimer RaftTimer
        {
            get;
            set;
        }

        void RaftTimerElapsed()
        {            
            InputQueue.Add("timer1");
        }

        static Random rnd = new Random();
        protected override void OnNodeCreated()
        {
            StartEventLoop();
            RaftTimer.SetElapsed(1000 + (rnd.Next() % 1000));
        }
        protected override bool OnNodeDeleting()
        {
            InputQueue.CompleteAdding();
            return true;
        }
        void StartEventLoop()
        {
            Thread t = new Thread(ProcessEvents);
            t.IsBackground = true;
            t.Start();
        }
        

        protected override void OnConnectionAdded(ConnectionViewModel connection)
        {
            IncrementAndSend();
        }
        public void Button1Click()
        {
            IncrementAndSend();
            RaftTimer.SetElapsed(1000 + rnd.Next(1666));
        }
        public void IncrementAndSend()
        {
            Broadcast(new Message() { Clock = ++Clock, Value = State });
        }
        private int _Clock;
        public int Clock
        {
            get { return _Clock; }
            set
            {
                if (_Clock != value)
                {
                    _Clock = value;
                    NotifyOfPropertyChange(() => Clock);
                }
            }
        }

        void ProcessEvents()
        {
            Console.Write("Started event queue worker");
            foreach(object evt in InputQueue.GetConsumingEnumerable())
            {
                // sttring event - timer elapsed name
                string timerName = evt as string;
                if (timerName != null)
                {
                    if (timerName == "timer1")
                    {
                       // Console.Beep();
                        RaftTimer.SetElapsed(2000);
                    }
                    continue;
                }

                if(evt != null)
                    Console.Write(evt.ToString());
                MessageReceived(null, evt);

                
            }

            Console.Write("Worker finished");
        }

        void MessageReceived(ConnectionViewModel connection, object message)
        {
            Message m = message as Message;

            if (Clock >= m.Clock)
                return;
            RaftTimer.SetElapsed(3000);

            Clock = m.Clock;
            Console.WriteLine(Name + " set value to " + m.Clock.ToString());
            State = m.Value;
            BroadcastExcept(m, connection);
        }

        private string _State;
        public string State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    NotifyOfPropertyChange(() => State);
                }
            }
        }
        

        
    }


}