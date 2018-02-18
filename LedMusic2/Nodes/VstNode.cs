using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes
{
    class VstNode : NodeBase
    {

        private NodeInterface<double> niValue;
        private NodeInterface<double> niNote;
        private NodeOptionViewModel channelOption = new NodeOptionViewModel(NodeOptionType.CUSTOM, "Channel", /* TODO */);

        public VstNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            niValue = AddOutput<double>("Value");
            niNote = AddOutput<double>("Note");

            //TODO: Add Node Option View (like OutputNode)

        }

        public override bool Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
