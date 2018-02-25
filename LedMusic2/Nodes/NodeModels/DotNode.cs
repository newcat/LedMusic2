using LedMusic2.LedColors;
using LedMusic2.NodeEditor;
using System;
using System.Windows;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Dot", NodeCategory.GENERATOR)]
    public class DotNode : NodeBase
    {

        public DotNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            AddInput("Center Position", 0.0);
            AddInput("Alpha", 1.0);
            AddInput<LedColors.LedColor>("Color", new LedColorRGB(0, 0, 0));
            AddInput("Glow", 0.0);
            AddInput("Symmetric", false);

            AddOutput<LedColors.LedColor[]>("Colors");

            Calculate();

        }

        public override bool Calculate()
        {

            int ledCount = GlobalProperties.Instance.LedCount;

            int centerPosition = (int)Clamp(
                ((NodeInterface<double>)Inputs.GetNodeInterface("Center Position")).Value, 0, ledCount);

            double alpha = Clamp(
                ((NodeInterface<double>)Inputs.GetNodeInterface("Alpha")).Value, 0, 1);

            double glow = Clamp(
                ((NodeInterface<double>)Inputs.GetNodeInterface("Glow")).Value, 0, ledCount);

            LedColorHSV color = ((NodeInterface<LedColors.LedColor>)Inputs.GetNodeInterface("Color")).Value.GetColorHSV();

            bool symmetric = ((NodeInterface<bool>)Inputs.GetNodeInterface("Symmetric")).Value;

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
                    buffer[i] = buffer[i].Add(reverseColors[i], 0.5).GetColorHSV();
                }
            }

            ((NodeInterface<LedColors.LedColor[]>)Outputs.GetNodeInterface("Colors")).SetValue(buffer);

            return true;

        }

        private double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

    }
}
