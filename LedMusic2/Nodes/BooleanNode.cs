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
    [Node("Boolean", NodeCategory.CONVERTER)]
    class BooleanNode : NodeBase
    {

        NodeOptionViewModel optOperation = new NodeOptionViewModel(NodeOptionType.SELECTION, "Operation");
        NodeOptionViewModel optInteger = new NodeOptionViewModel(NodeOptionType.BOOL, "Use integer values");
        NodeOptionViewModel optInvert = new NodeOptionViewModel(NodeOptionType.BOOL, "Invert output");

        public BooleanNode(Point initPosition) : base(initPosition)
        {

            Inputs.Add(new NodeInterface<double>("Value 1", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<double>("Value 2", ConnectionType.NUMBER, this, true));

            foreach (string s in new string[] { "==", ">", "<", ">=", "<=" })
            {
                optOperation.Options.Add(s);
            }
            optOperation.DisplayValue = "==";
            Options.Add(optOperation);
            Options.Add(optInteger);
            Options.Add(optInvert);

            Outputs.Add(new NodeInterface<bool>("Output", ConnectionType.BOOL, this, false));

        }

        public override bool Calculate()
        {

            bool result = false;
            bool useInt = (bool)optInteger.RenderValue;

            double val1 = ((NodeInterface<double>)Inputs[0]).Value;
            double val2 = ((NodeInterface<double>)Inputs[1]).Value;

            switch ((string)optOperation.RenderValue)
            {
                case "==":
                    result = useInt ? (int)val1 == (int)val2 : val1 == val2;
                    break;
                case ">":
                    result = useInt ? (int)val1 > (int)val2 : val1 > val2;
                    break;
                case "<":
                    result = useInt ? (int)val1 < (int)val2 : val1 < val2;
                    break;
                case ">=":
                    result = useInt ? (int)val1 >= (int)val2 : val1 >= val2;
                    break;
                case "<=":
                    result = useInt ? (int)val1 <= (int)val2 : val1 <= val2;
                    break;
            }

            if ((bool)optInvert.RenderValue)
                result = !result;

            Outputs[0].SetValue(result);

            return true;

        }
    }
}
