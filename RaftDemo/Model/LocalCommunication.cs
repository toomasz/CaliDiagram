using CaliDiagram.ViewModels;
using RaftDemo.Raft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public class LocalCommunication: ICommuncatuionModel
    {
        SimulationSettings worldSettings;
        public LocalCommunication(SimulationSettings worldSettings)
        {
            this.worldSettings = worldSettings;
        }
        INodeChannel ICommuncatuionModel.CreateChannel(ConnectionViewModel connection, NodeBaseViewModel from)
        {
            return new NodeChannel(connection, from, worldSettings);
        }
    }
}
