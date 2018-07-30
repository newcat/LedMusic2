using LedMusic2.LedColors;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Numeric value", NodeCategory.INPUT)]
    class DoubleValueNode : NodeBase
    {

        NodeInterface<double> outputInterface;
        NodeOption opt = new NodeOption(NodeOptionType.NUMBER, "Value");

        public DoubleValueNode() : base()
        {

            Options.Add(opt);
            outputInterface = AddOutput<double>("Value");         

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.Value.Get());
            return true;
        }

    }

    [Node("Boolean value", NodeCategory.INPUT)]
    class BoolValueNode : NodeBase
    {

        NodeInterface<bool> outputInterface;
        NodeOption opt = new NodeOption(NodeOptionType.BOOL, "Value");

        public BoolValueNode() : base()
        {

            Options.Add(opt);
            outputInterface = AddOutput<bool>("Value");

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.Value.Get());
            return true;
        }

    }

    [Node("Color value", NodeCategory.INPUT)]
    class ColorValueNode : NodeBase
    {

        NodeInterface<LedColors.LedColor> outputInterface;
        NodeOption opt = new NodeOption(NodeOptionType.COLOR, "Value");

        public ColorValueNode() : base()
        {

            Options.Add(opt);
            outputInterface = AddOutput<LedColor>("Value");

        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.Value.Get());
            return true;
        }

    }


}
