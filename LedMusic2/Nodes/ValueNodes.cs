using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Numeric value", NodeCategory.INPUT)]
    class DoubleValueNode : NodeBase
    {

        NodeInterface<double> outputInterface;
        NodeOptionViewModel opt = new NodeOptionViewModel(NodeOptionType.NUMBER, "Value");

        public DoubleValueNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            MinWidth = 150;

            Options.Add(opt);
            outputInterface = AddOutput<double>("Value");         

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

        public BoolValueNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            Options.Add(opt);
            outputInterface = AddOutput<bool>("Value");

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

        public ColorValueNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            MinWidth = 125;

            Options.Add(opt);
            outputInterface = AddOutput<LedColor>("Value");

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.RenderValue);
            return true;
        }

    }


}
