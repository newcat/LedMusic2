using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Math", NodeCategory.CONVERTER)]
    class MathNode : NodeBase
    {

        NodeOptionViewModel optOperation;
        NodeOptionViewModel optClamp;

        public MathNode(Point initPosition) : base(initPosition)
        {

            _inputs.Add(new NodeInterface<double>("Value 1", ConnectionType.NUMBER, this, true, 0));
            _inputs.Add(new NodeInterface<double>("Value 2", ConnectionType.NUMBER, this, true, 0));

            _outputs.Add(new NodeInterface<double>("Output", ConnectionType.NUMBER, this, false));

            optOperation = new NodeOptionViewModel(NodeOptionType.SELECTION, "Operation");
            foreach (string s in new string[] { "Add", "Subtract", "Multiply", "Divide", "Sine", "Cosine", "Tangent", "Arcsine", "Arccosine",
                                                "Arctangent", "Power", "Logarithm", "Minimum", "Maximum", "Round", "Modulo", "Absolute"})
            {
                optOperation.Options.Add(s);
            }
            optOperation.Value = "Add";
            optOperation.PropertyChanged += Option_PropertyChanged;
            _options.Add(optOperation);

            optClamp = new NodeOptionViewModel(NodeOptionType.BOOL, "Clamp");
            optClamp.PropertyChanged += Option_PropertyChanged;
            _options.Add(optClamp);

            Calculate();

        }

        public override bool Calculate()
        {

            double val1 = ((NodeInterface<double>)_inputs.GetNodeInterface("Value 1")).Value;
            double val2 = ((NodeInterface<double>)_inputs.GetNodeInterface("Value 2")).Value;

            ((NodeInterface<double>)_outputs.GetNodeInterface("Output")).SetValue(val1 + val2);
            return true;

        }

        private void Option_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                InvokeOutputChanged();
        }

    }
}
