using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Color Ramp", NodeCategory.COLOR)]
    class ColorRampNode : NodeBase, INotifyPropertyChanged
    {

        public string ButtonName { get { return "SuperButton"; } }

        public ColorRampNode(Point initPosition) : base(initPosition)
        {

            _outputs.Add(new NodeInterface<Color[]>("Color", ConnectionType.COLOR_ARRAY, this, false));

            _options.Add(new NodeOptionViewModel(NodeOptionType.CUSTOM, "Test", typeof(NodeViews.ColorRampNode), this));

        }

        public override bool Calculate()
        {
            return true;
        }
    }
}
