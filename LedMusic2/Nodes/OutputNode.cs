using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.Outputs;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public OutputNode(Point initPosition) : base(initPosition)
        {

            MinWidth = 175;

            input = new NodeInterface<LedColor[]>("Color", ConnectionType.COLOR_ARRAY, this, true);
            Inputs.Add(input);

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


    }
}
