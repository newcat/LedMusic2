using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Numeric value", NodeCategory.INPUT)]
    class DoubleValueNode : NodeBase
    {

        NodeInterface<double> outputInterface;
        NodeOptionViewModel opt = new NodeOptionViewModel(NodeOptionType.NUMBER, "Value");

        public DoubleValueNode(Point initPosition) : base(initPosition)
        {

            MinWidth = 150;

            _options.Add(opt);

            outputInterface = new NodeInterface<double>("Value", ConnectionType.NUMBER, this, false);
            _outputs.Add(outputInterface);            

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.RenderValue);
            return true;
        }

    }

    [Node("Boolean value", NodeCategory.INPUT)]
    class BoolValueNode : NodeBase
    {

        NodeInterface<bool> outputInterface;
        NodeOptionViewModel opt = new NodeOptionViewModel(NodeOptionType.BOOL, "Value");

        public BoolValueNode(Point initPosition) : base(initPosition)
        {

            _options.Add(opt);

            outputInterface = new NodeInterface<bool>("Value", ConnectionType.BOOL, this, false);
            _outputs.Add(outputInterface);

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.RenderValue);
            return true;
        }

    }

    [Node("Color value", NodeCategory.INPUT)]
    class ColorValueNode : NodeBase
    {

        NodeInterface<LedColor> outputInterface;
        NodeOptionViewModel opt = new NodeOptionViewModel(NodeOptionType.COLOR, "Value");

        public ColorValueNode(Point initPosition) : base(initPosition)
        {

            MinWidth = 125;

            _options.Add(opt);

            outputInterface = new NodeInterface<LedColor>("Value", ConnectionType.COLOR, this, false);
            _outputs.Add(outputInterface);

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.RenderValue);
            return true;
        }

    }


}
