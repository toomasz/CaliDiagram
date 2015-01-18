using DiagramLib.Model;
using System.Runtime.Serialization;

namespace DiagramDesigner.Model
{
    class Message
    {

    }
    class Channel
    {

    }
    [DataContract]
    class DiagramNodeBig:DiagramNodeBase
    {
        
        void OnMessageReceived(Channel c, Message m)
        {
            BroadcastExcept(m, c);
        }
        void OnChannelAdded(Channel c)
        {
            SendMessage(c, new Message());
        }
        void OnChannelRemoved(Channel c)
        {
            
        }
        void BroadcastMessage(Message m)
        {

        }
        void BroadcastExcept(Message m, Channel except)
        {

        }
        void SendMessage(Channel c, Message m)
        {

        }
        void DiscardMessage(Message m)
        {

        }
    }
}
