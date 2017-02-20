using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Sound;
using LedMusic2.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

namespace LedMusic2.Nodes
{
    [Node("Beat", NodeCategory.INPUT)]
    class BeatNode : NodeBase
    {
        
        public BeatNode(Point initPosition) : base(initPosition)
        {

            _outputs.Add(new NodeInterface<double>("Beat", ConnectionType.NUMBER, this, false));

        }

        public override bool Calculate()
        {
            _outputs[0].SetValue((double)SoundEngine.Instance.BeatDetector.GetBeatValue(MainViewModel.Instance.CurrentFrame));
            return true;
        }
    }
}
