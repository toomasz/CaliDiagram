using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel
{
    public class MessageReceivedArgs
    {
        public MessageReceivedArgs(INetworkSocket socket, object message)
        {
            this.Socket = socket;
            this.Message = message;
        }
        public object Message { get; private set;}
        public INetworkSocket Socket {get; private set;}
    }
    public interface INetworkServer: IDisposable
    {
        /// <summary>
        /// Start accepting connections
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool StartListening(string address);

        /// <summary>
        /// Stop listening in current address
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// Listening address
        /// </summary>
        string Address { get; }

        /// <summary>
        /// List of all client channels
        /// </summary>
        IList<INetworkSocket> ClientChannels { get; }

        /// <summary>
        /// Listening channel
        /// </summary>
        INetworkSocket ListeningChannel { get; }
        /// <summary>
        /// New client connected to server
        /// </summary>
        event EventHandler<INetworkSocket> ClientConnected;
        /// <summary>
        /// Connection to server was closed
        /// </summary>
        event EventHandler<INetworkSocket> ClientDisconnected;

        /// <summary>
        /// Fired when message from any client is received
        /// </summary>
        event EventHandler<MessageReceivedArgs> MessageReceived;
    }
}
