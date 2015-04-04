using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.InProcNetwork
{
    class SocketId: IEquatable<SocketId>
    {
        /// <summary>
        /// Get socket id of socket [local_address, remote_address]
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static SocketId FromSocket(InProcSocket socket)
        {
            return new SocketId(socket.LocalAddress, socket.RemoteAddress);
        }
        /// <summary>
        /// Get id of associated remote socket [remote_address, local_address]
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static SocketId RemoteSocketId(InProcSocket socket)
        {
            return new SocketId(socket.RemoteAddress, socket.LocalAddress);
        }
        public SocketId(string localAddress, string remoteAddress)
        {
            if (string.IsNullOrEmpty(localAddress) || string.IsNullOrEmpty(remoteAddress))
                throw new ArgumentException("Empty address passed to SocketId constructor");
            this.LocalAddress = localAddress;
            this.RemoteAddress = remoteAddress;
        }
        public string LocalAddress { get; private set; }
        public string RemoteAddress { get; private set; }
        public override bool Equals(object obj)
        {
            var other = obj as SocketId;
            return Equals(other);
        }
        public override string ToString()
        {
            return string.Format("{0} => {1}", LocalAddress, RemoteAddress);
        }

        public bool Equals(SocketId other)
        {
            if (other == null)
                return false;
            return other.LocalAddress == LocalAddress && other.RemoteAddress == RemoteAddress;
        }
        public override int GetHashCode()
        {
            return LocalAddress.GetHashCode() ^ RemoteAddress.GetHashCode();
        }
    }
}
