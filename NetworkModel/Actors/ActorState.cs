using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkModel.Actors
{
    public enum ActorState
    {
        /// <summary>
        /// Actor is stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// Actor is booting up
        /// </summary>
        Starting,
        /// <summary>
        /// Actor is running
        /// </summary>
        Started,
        /// <summary>
        /// Actor is shutting down
        /// </summary>
        Stopping,
        /// <summary>
        /// Unrecoverable error occured during actor operation
        /// </summary>
        Error
    }
}
