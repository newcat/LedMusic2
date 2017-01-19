using LedMusic2.Attributes;
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

    [Node("Output", NodeCategory.OUTPUT)]
    public class OutputNode : NodeBase
    {

        private NodeInterface<LedColor[]> input;
        private NodeOptionViewModel preview = new NodeOptionViewModel(NodeOptionType.PREVIEW, "Preview");

        public OutputNode(Point initPosition) : base(initPosition)
        {

            MinWidth = 125;

            input = new NodeInterface<LedColor[]>("Color", ConnectionType.COLOR_ARRAY, this, true);
            _inputs.Add(input);

            _options.Add(preview);
        }

        public override bool Calculate()
        {
            preview.Value = input.Value;
            return true;
        }


    }
}
