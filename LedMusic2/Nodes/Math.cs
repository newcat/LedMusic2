using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
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
            double outputVal = 0;

            switch ((string)optOperation.Value)
            {
                case "Add":
                    outputVal = val1 + val2;
                    break;
                case "Subtract":
                    outputVal = val1 - val2;
                    break;
                case "Multiply":
                    outputVal = val1 * val2;
                    break;
                case "Divide":
                    outputVal = val2 != 0 ? val1 / val2 : 0;
                    break;
                case "Sine":
                    outputVal = Math.Sin(val1);
                    break;
                case "Cosine":
                    outputVal = Math.Cos(val1);
                    break;
                case "Tangent":
                    outputVal = Math.Tan(val1);
                    break;
                case "Arcsine":
                    outputVal = Math.Asin(val1);
                    break;
                case "Arccosine":
                    outputVal = Math.Acos(val1);
                    break;
                case "Arctangent":
                    outputVal = Math.Atan(val1);
                    break;
                case "Power":
                    outputVal = Math.Pow(val1, val2);
                    break;
                case "Logarithm":
                    outputVal = Math.Log(val1, val2);
                    break;
                case "Minimum":
                    outputVal = Math.Min(val1, val2);
                    break;
                case "Maximum":
                    outputVal = Math.Max(val1, val2);
                    break;
                case "Round":
                    outputVal = Math.Round(val1);
                    break;
                case "Modulo":
                    outputVal = val1 % val2;
                    break;
                case "Absolute":
                    outputVal = Math.Abs(val1);
                    break;
                default:
                    outputVal = 0;
                    break;
            }


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
