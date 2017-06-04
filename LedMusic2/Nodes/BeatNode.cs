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

            Outputs.Add(new NodeInterface<double>("Beat", ConnectionType.NUMBER, this, false));

            Inputs.Add(new NodeInterface<bool>("Custom Beat", ConnectionType.BOOL, this, true));
            Inputs.Add(new NodeInterface<double>("BPM", ConnectionType.NUMBER, this, true));
            Inputs.Add(new NodeInterface<double>("Offset", ConnectionType.NUMBER, this, true));

        }

        public override bool Calculate()
        {

            if (((NodeInterface<bool>)Inputs.GetNodeInterface("Custom Beat")).Value)
            {
                Outputs[0].SetValue(getBeatValue(MainViewModel.Instance.CurrentFrame));
            } else
            {
                Outputs[0].SetValue((double)SoundEngine.Instance.BeatDetector.GetBeatValue(MainViewModel.Instance.CurrentFrame));
            }
            
            return true;
        }

        private double getBeatValue(int frame)
        {

            double bpm = ((NodeInterface<double>)Inputs.GetNodeInterface("BPM")).Value;
            double offset = ((NodeInterface<double>)Inputs.GetNodeInterface("Offset")).Value;

            if (bpm == 0.0)
            {
                return 0.0;
            }

            double bps = bpm / 60.0;
            double spb = 1.0 / bps;
            double time = (frame * 1.0 / GlobalProperties.Instance.FPS) - (offset * spb);            

            int lastBeatNumber = (int)Math.Floor(time * bps);
            double lastBeatTime = lastBeatNumber * spb;
            return 1f - (time - lastBeatTime) / spb;

        }

    }
}
