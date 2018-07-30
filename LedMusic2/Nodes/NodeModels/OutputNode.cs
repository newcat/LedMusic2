using LedMusic2.LedColors;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Output", NodeCategory.OUTPUT)]
    public class OutputNode : NodeBase
    {

        private NodeInterface<LedColor[]> input;
        private NodeOption preview = new NodeOption(NodeOptionType.PREVIEW, "Preview");
        private readonly NodeOption outputSelection;

        public OutputBase SelectedOutput { get; set; }

        public OutputNode() : base()
        {

            input = AddInput<LedColor[]>("Color");

            Options.Add(preview);

            outputSelection = new NodeOption(NodeOptionType.CUSTOM, "Output", typeof(OutputSelection), this);
            Options.Add(outputSelection);

        }

        public override bool Calculate()
        {
            preview.Value.Set(input.Value);
            SelectedOutput?.CalculationDone(input.Value);
            return true;
        }

        protected override void SaveAdditionalXmlData(XElement x)
        {
            x.Add(new XElement("output", SelectedOutput?.Id));
        }

        protected override void LoadAdditionalXmlData(XElement x)
        {
            foreach (var el in x.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "output":
                        foreach (var o in MainViewModel.Instance.OutputManager.Outputs)
                        {
                            if (o.Id.ToString() == el.Value)
                            {
                                SelectedOutput = o;
                                break;
                            }
                        }
                        break;
                }
            }
        }


    }
}
