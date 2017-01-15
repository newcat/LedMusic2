using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Math", NodeCategory.CONVERTER)]
    class MathNode : NodeBase
    {

        public MathNode(Point initPosition) : base(initPosition)
        {

            _inputs.Add(new NodeInterface<double>("Value 1", ConnectionType.NUMBER, this, true, 0));
            _inputs.Add(new NodeInterface<double>("Value 2", ConnectionType.NUMBER, this, true, 0));

            _outputs.Add(new NodeInterface<double>("Output", ConnectionType.NUMBER, this, false));

            Calculate();

        }

        public override bool Calculate()
        {

            double val1 = ((NodeInterface<double>)_inputs.GetNodeInterface("Value 1")).Value;
            double val2 = ((NodeInterface<double>)_inputs.GetNodeInterface("Value 2")).Value;

            ((NodeInterface<double>)_outputs.GetNodeInterface("Output")).SetValue(val1 + val2);
            return true;

        }

        protected override bool InputValueChanged(string NodeInterfaceName)
        {
            return true;
        }
    }
}
