using LedMusic2.Reactive;
using LedMusic2.Nodes.NodeOptions;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeModels
{
    [Node("Boolean", NodeCategory.CONVERTER)]
    class BooleanNode : NodeBase
    {

        SelectOption<ReactiveListItem<string>> optOperation = new SelectOption<ReactiveListItem<string>>("Operation");
        BoolOption optInteger = new BoolOption("Use integer values");
        BoolOption optInvert = new BoolOption("Invert output");

        public BooleanNode()
        {

            AddInput("Value 1", 0.0);
            AddInput("Value 2", 0.0);

            var operationOptions = new ReactiveCollection<ReactiveListItem<string>>();
            foreach (string s in new string[] { "==", ">", "<", ">=", "<=" })
            {
                operationOptions.Add(new ReactiveListItem<string>(s));
            }
            optOperation.SetOptions(operationOptions);
            optOperation.SelectedId.Set(operationOptions[0].Id.ToString());
            Options.Add(optOperation);
            Options.Add(optInteger);
            Options.Add(optInvert);

            AddOutput<bool>("Output");

        }

        public BooleanNode(JToken j) : this()
        {
            LoadState(j);
        }

        public override bool Calculate()
        {

            bool result = false;
            bool useInt = optInteger.Value.Get();

            double val1 = ((NodeInterface<double>)Inputs[0]).Value;
            double val2 = ((NodeInterface<double>)Inputs[1]).Value;

            switch (optOperation.Value.Get())
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

            if (optInvert.Value.Get())
                result = !result;

            Outputs[0].SetValue(result);

            return true;

        }
    }
}
