using LedMusic2.LedColors;
using LedMusic2.Nodes.NodeOptions;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Output", NodeCategory.OUTPUT)]
    public class OutputNode : NodeBase
    {

        private NodeInterface<LedColor[]> input;
        private PreviewOption preview = new PreviewOption("Preview");
        private SelectOption<OutputBase> outputSelection = new SelectOption<OutputBase>("Output");

        public OutputNode() : base()
        {

            input = AddInput<LedColor[]>("Color");

            Options.Add(preview);
            Options.Add(outputSelection);
            outputSelection.SetOptions(MainViewModel.Instance.OutputManager.Outputs);
            outputSelection.ItemDisplayPropertyName.Set("Name");

        }

        public override bool Calculate()
        {
            preview.Value.Set(input.Value);
            outputSelection.Value?.CalculationDone(input.Value);
            return true;
        }

    }
}
