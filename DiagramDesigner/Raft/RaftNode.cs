using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner.Raft
{
    public class Message
    {
        public int Clock { get; set; }
        public string State { get; set; }
        public override string ToString()
        {
            return string.Format("MSG: {0} {1}", Clock, State);
        }
    }
    public class ResponseMessage
    {
        
    }
    public class RaftNode : NetworkSoftwareBase
    {
        public RaftNode(ICommunication communicationModel):base(communicationModel)
        {
            Clock = 0;
        }

        Random rnd = new Random();
        NodeTimer raftTimer;

        protected override void OnInitialized()
        {
            raftTimer = new NodeTimer(this);
            raftTimer.SetElapseIn(1000);
            foreach(var c in Channels)
            {

            }
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
        

        public void IncrementAndSend()
        {
            Message message = new Message() { Clock = ++Clock, State = this.State };
            BroadcastMessage(message);            
        }
        public void Button1Click()
        {
            IncrementAndSend();
            raftTimer.SetElapseIn(1000 + rnd.Next(1666));
        }

        protected override void OnDestroyed()
        {
          //  Console.Beep();
        }
        protected override void OnChannelCreated(INodeChannel channel)
        {
            SendMessage(channel, "Hello you !");
        }
        protected override void OnChannelDestroyed(INodeChannel channel)
        {
            
        }

        protected override void OnMessageReceived(INodeChannel channel, object message)
        {
            Message msg = message as Message;
            if (msg != null)
            {           
                SendMessage(channel, new ResponseMessage());
                if (this.Clock >= msg.Clock)
                    return;
                this.Clock = msg.Clock;
                this.State = msg.State;
                BroadcastExcept(msg, channel);
            }
         /*   ResponseMessage response = message as ResponseMessage;
            if (response != null)
            {
                Clock += 2;
            }*/
        }

        protected override void OnCommandReceived(string command)
        {
            if (command == "send")
                IncrementAndSend();
            
        }
        protected void OnTimerReset(NodeTimer timer)
        {
            
        }
        public override string ToString()
        {
            return "Raft software";
        }
    }
}
