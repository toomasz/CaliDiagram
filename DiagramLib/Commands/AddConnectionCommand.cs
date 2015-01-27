using DiagramLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramLib.Commands
{
    class AddConnectionCommand: DiagramCommand
    {
        public AddConnectionCommand(DiagramViewModel diagram)
            : base(diagram)
        {
            Description = "Add connection";
        }
        public override string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Most important random generator in whole sofrware
        /// </summary>
        Random rndStr = new Random();

        string RandomNextStr
        {
            get
            {
                return nextOneStr[rndStr.Next(nextOneStr.Length)];
            }
        }

        string[] nextOneStr = 
        {
            "Want to add another one? Select start node again.",
            "Wow, you are getting better at this",
            "Another one?",
            "One more?",
            "Please select start node for connection",
            "More connections?",
        };

        private NodeBaseViewModel prevSelectedNode = null;
        public override void HandleNodeClick(NodeBaseViewModel node)
        {
            if (prevSelectedNode != null)
            {
                Diagram.AddConnection(prevSelectedNode, node);
                prevSelectedNode = null;
                Diagram.HelpText = RandomNextStr;
            }
            else
            {
                Diagram.HelpText = "Great ! Now select destination";
                prevSelectedNode = node;
            }
            
        }

        public override void HandleConnectionClick(ConnectionViewModel node)
        {
            
        }

        public override void HandleDiagramClick(System.Windows.Point location)
        {
            
        }
    }
}
