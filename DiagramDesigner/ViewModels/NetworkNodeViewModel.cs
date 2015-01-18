using DiagramLib.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System;
using System.Threading;

namespace DiagramDesigner.ViewModels
{
    class PacketModel
    {
        public string Caption
        {
            get;
            set;
        }
        public NodeBaseViewModel To { get; set; }
    }
    public class NetworkNodeViewModel: NodeBaseViewModel
    {
        public NetworkNodeViewModel()
        {
            InputQueue = new BlockingCollection<object>();
            
        }

       

        public BlockingCollection<object> InputQueue
        {
            get;
            private set;
        }

        public int GetDelay(ConnectionViewModel connection)
        {
            return 600;
        }

        public void Broadcast(object message)
        {
            foreach (var conn in Connections)
            {
                SendMessage(conn, message, GetDelay(conn));
            }
        }
        
        public event EventHandler<object> PacketSent;



        void ClockElapsed()
        {
            
        }
    
        public void BroadcastExcept(object message, ConnectionViewModel except)
        {
            foreach (var conn in Connections)
            {
                if (conn == except)
                    continue;
                SendMessage(conn, message, GetDelay(conn));
            }
        }
        public async Task SendMessage(ConnectionViewModel connection, object message, int delay)
        {
           
            DiagramNodeBigViewModel to = null;
            if (connection.From == this)
                to = (DiagramNodeBigViewModel)connection.To;
            else if (connection.To == this)
                to = (DiagramNodeBigViewModel)connection.From;
            else
                throw new ArgumentException();

            PacketModel packet = new PacketModel() { Caption = message.ToString(), To = to };
            if (PacketSent != null)
                PacketSent(this, packet);

            await Task.Delay(delay);
            to.InputQueue.Add(message);
           // to.OnMessageReceived(connection, message);
        }


        protected override void OnNodeCreated()
        {
            Console.WriteLine(Name + " Created");
            Console.WriteLine(Connections.Count.ToString());
        }
        protected override bool OnNodeDeleting()
        {
            Console.WriteLine(Name + " removed");
            return true;
        }
        protected override void OnConnectionAdded(ConnectionViewModel connection)
        {
           
        }
        protected override void OnConnectionRemoved(ConnectionViewModel connection)
        {
            
        }
    }
}
