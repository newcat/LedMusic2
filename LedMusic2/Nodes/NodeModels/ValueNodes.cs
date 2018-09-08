using LedMusic2.LedColors;
using LedMusic2.Nodes.NodeOptions;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Numeric value", NodeCategory.INPUT)]
    class DoubleValueNode : NodeBase
    {

        NodeInterface<double> outputInterface;
        NumberOption opt = new NumberOption("Value");

        public DoubleValueNode()
        {
            Options.Add(opt);
            outputInterface = AddOutput<double>("Value");
        }

        public DoubleValueNode(JToken j) : this()
        {
            LoadState(j);
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
        BoolOption opt = new BoolOption("Value");

        public BoolValueNode() : base()
        {
            Options.Add(opt);
            outputInterface = AddOutput<bool>("Value");
        }

        public BoolValueNode(JToken j) : this()
        {
            LoadState(j);
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

        NodeInterface<LedColor> outputInterface;
        ColorOption opt = new ColorOption("Value");

        public ColorValueNode()
        {
            Options.Add(opt);
            outputInterface = AddOutput<LedColor>("Value");
            outputInterface.SetValue(opt.Value.Get());
        }

        public ColorValueNode(JToken j) : this()
        {
            LoadState(j);
        }

        public override bool Calculate()
        {
            outputInterface.SetValue(opt.Value.Get());
            return true;
        }

    }


}
