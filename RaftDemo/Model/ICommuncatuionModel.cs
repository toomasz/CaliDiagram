using CaliDiagram.ViewModels;
using RaftDemo.Raft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftDemo.Model
{
    public interface ICommuncatuionModel
    {
        INodeChannel CreateChannel(ConnectionViewModel connection, NodeBaseViewModel from);
    }
}
