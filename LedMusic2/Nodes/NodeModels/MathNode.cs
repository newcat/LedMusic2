using LedMusic2.NodeEditor;
using System;
using System.Windows;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Math", NodeCategory.CONVERTER)]
    class MathNode : NodeBase
    {

        NodeOption optOperation;
        NodeOption optClamp;

        public MathNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            MinWidth = 150;

            AddInput("Value 1", 0.0);
            AddInput("Value 2", 0.0);
            AddOutput<double>("Output");

            optOperation = new NodeOption(NodeOptionType.SELECTION, "Operation");
            foreach (string s in new string[] { "Add", "Subtract", "Multiply", "Divide", "Sine", "Cosine", "Tangent", "Arcsine", "Arccosine",
                                                "Arctangent", "Power", "Logarithm", "Minimum", "Maximum", "Round", "Modulo", "Absolute"})
            {
                optOperation.Options.Add(s);
            }
            optOperation.DisplayValue = "Add";
            _options.Add(optOperation);

            optClamp = new NodeOption(NodeOptionType.BOOL, "Clamp");
            _options.Add(optClamp);

            Calculate();

        }

        public override bool Calculate()
        {

            double val1 = ((NodeInterface<double>)_inputs.GetNodeInterface("Value 1")).Value;
            double val2 = ((NodeInterface<double>)_inputs.GetNodeInterface("Value 2")).Value;
            double outputVal = 0;

            switch ((string)optOperation.RenderValue)
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

            if ((bool)optClamp.RenderValue)
                outputVal = Math.Max(0, Math.Min(1, outputVal));

            ((NodeInterface<double>)_outputs.GetNodeInterface("Output")).SetValue(outputVal);
            return true;

        }

    }
}
