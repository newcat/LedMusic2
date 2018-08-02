using LedMusic2.LedColors;
using LedMusic2.Nodes.NodeOptions;
using LedMusic2.Outputs;
using LedMusic2.Reactive;
using LedMusic2.ViewModels;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Output", NodeCategory.OUTPUT)]
    public class OutputNode : NodeBase
    {

        private NodeInterface<LedColor[]> input;
        private PreviewOption preview = new PreviewOption("Preview");
        private SelectOption outputSelection = new SelectOption("Output");

        [ReactiveIgnore]
        public OutputBase SelectedOutput { get; set; }

        public OutputNode() : base()
        {

            input = AddInput<LedColor[]>("Color");

            Options.Add(preview);
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
