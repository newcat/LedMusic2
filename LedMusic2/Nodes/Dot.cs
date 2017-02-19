﻿using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Windows;

namespace LedMusic2.Nodes
{

    [Node("Dot", NodeCategory.GENERATOR)]
    public class Dot : NodeBase
    {

        public Dot(Point initPosition) : base(initPosition)
        {

            _inputs.Add(new NodeInterface<double>("Center Position", ConnectionType.NUMBER, this, true, 0));
            _inputs.Add(new NodeInterface<double>("Alpha", ConnectionType.NUMBER, this, true, 1));
            _inputs.Add(new NodeInterface<LedColor>("Color", ConnectionType.COLOR, this, true, new LedColorRGB(0, 0, 0)));
            _inputs.Add(new NodeInterface<double>("Glow", ConnectionType.NUMBER, this, true, 0));
            _inputs.Add(new NodeInterface<bool>("Symmetric", ConnectionType.BOOL, this, true, false));

            _outputs.Add(new NodeInterface<LedColor[]>("Colors", ConnectionType.COLOR_ARRAY, this, false));

            Calculate();

        }

        public override bool Calculate()
        {

            int ledCount = GlobalProperties.Instance.LedCount;

            int centerPosition = (int)clamp(
                ((NodeInterface<double>)_inputs.GetNodeInterface("Center Position")).Value, 0, ledCount);

            double alpha = clamp(
                ((NodeInterface<double>)_inputs.GetNodeInterface("Alpha")).Value, 0, 1);

            double glow = clamp(
                ((NodeInterface<double>)_inputs.GetNodeInterface("Glow")).Value, 0, ledCount);

            LedColorHSV color = ((NodeInterface<LedColor>)_inputs.GetNodeInterface("Color")).Value.GetColorHSV();

            bool symmetric = ((NodeInterface<bool>)_inputs.GetNodeInterface("Symmetric")).Value;

            LedColorHSV[] buffer = new LedColorHSV[ledCount];

            for (int i = (int)Math.Floor(centerPosition - glow); i <= (int)Math.Ceiling(centerPosition + glow); i++)
            {
                if (i >= 0 && i < ledCount)
                    buffer[i] = new LedColorHSV(
                        color.H, color.S, alpha * Math.Max((-Math.Abs(i - centerPosition)) / (glow + 1) + 1, 0) * color.V);
            }

            if (symmetric)
            {
                LedColorHSV[] reverseColors = new LedColorHSV[ledCount];
                for (int i = 0; i < ledCount; i++)
                {
                    if (buffer[i] == null)
                        buffer[i] = new LedColorHSV(0, 0, 0);
                    reverseColors[ledCount - i - 1] = buffer[i];
                }
                for (int i = 0; i < ledCount; i++)
                {
                    buffer[i] = buffer[i].Add(reverseColors[i]);
                }
            }

            ((NodeInterface<LedColor[]>)_outputs.GetNodeInterface("Colors")).SetValue(buffer);

            return true;

        }

        private double clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

    }
}
