using LedMusic2.Color;
using LedMusic2.NodeEditor;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Output", NodeCategory.OUTPUT)]
    public class OutputNode : NodeBase
    {

        private NodeInterface<LedColor[]> input;
        private NodeOption preview = new NodeOption(NodeOptionType.PREVIEW, "Preview");
        private NodeOption outputSelection;

        private OutputBase _selectedOutput;
        public OutputBase SelectedOutput
        {
            get { return _selectedOutput; }
            set
            {
                _selectedOutput = value;
                NotifyPropertyChanged();
            }
        }

        public OutputNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            MinWidth = 175;

            input = AddInput<LedColor[]>("Color");

            Options.Add(preview);

            outputSelection = new NodeOption(NodeOptionType.CUSTOM, "Output", typeof(OutputSelection), this);
            Options.Add(outputSelection);

        }

        public override bool Calculate()
        {
            preview.DisplayValue = input.Value;
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
                        foreach (var o in MainViewModel.Instance.Outputs)
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
