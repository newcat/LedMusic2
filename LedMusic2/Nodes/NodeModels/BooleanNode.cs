using LedMusic2.NodeEditor;
using System.Windows;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Boolean", NodeCategory.CONVERTER)]
    class BooleanNode : NodeBase
    {

        NodeOption optOperation = new NodeOption(NodeOptionType.SELECTION, "Operation");
        NodeOption optInteger = new NodeOption(NodeOptionType.BOOL, "Use integer values");
        NodeOption optInvert = new NodeOption(NodeOptionType.BOOL, "Invert output");

        public BooleanNode() : base()
        {

            AddInput("Value 1", 0.0);
            AddInput("Value 2", 0.0);

            foreach (string s in new string[] { "==", ">", "<", ">=", "<=" })
            {
                optOperation.Options.Add(s);
            }
            optOperation.Value.Set("==");
            Options.Add(optOperation);
            Options.Add(optInteger);
            Options.Add(optInvert);

            AddOutput<bool>("Output");

        }

        public override bool Calculate()
        {

            bool result = false;
            bool useInt = (bool)optInteger.Value.Get();

            double val1 = ((NodeInterface<double>)Inputs[0]).Value;
            double val2 = ((NodeInterface<double>)Inputs[1]).Value;

            switch ((string)optOperation.Value.Get())
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

            if ((bool)optInvert.Value.Get())
                result = !result;

            Outputs[0].SetValue(result);

            return true;

        }
    }
}
