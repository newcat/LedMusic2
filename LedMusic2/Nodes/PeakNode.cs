using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Sound;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Peak", NodeCategory.INPUT)]
    class PeakNode : NodeBase
    {
        public PeakNode(Point initPosition) : base(initPosition)
        {
            _outputs.Add(new NodeInterface<double>("Peak", ConnectionType.NUMBER, this, false));
        }

        public override bool Calculate()
        {
            _outputs[0].SetValue((double)SoundEngine.Instance.GetCurrentSample());
            return true;
        }
    }
}
