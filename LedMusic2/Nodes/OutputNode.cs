using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{

    [Node("Output", NodeCategory.OUTPUT)]
    public class OutputNode : NodeBase
    {

        private NodeInterface<LedColor[]> input;
        private NodeOptionViewModel preview = new NodeOptionViewModel(NodeOptionType.PREVIEW, "Preview");
        private NodeOptionViewModel outputSelection;

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

            outputSelection = new NodeOptionViewModel(NodeOptionType.CUSTOM, "Output", typeof(OutputSelection), this);
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
