using LedMusic2.LedColors;
using LedMusic2.Nodes.NodeOptions;
using LedMusic2.Reactive;
using System;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Dot", NodeCategory.GENERATOR)]
    public class DotNode : NodeBase
    {

        private NodeInterface<double> niCenterPosition;
        private NodeInterface<double> niAlpha;
        private NodeInterface<LedColor> niColor;
        private NodeInterface<double> niGlow;
        private NodeInterface<bool> niSymmetric;
        private NodeInterface<LedColor[]> niOutput;

        private SelectOption<ReactiveListItem<string>> noGlowType = new SelectOption<ReactiveListItem<string>>("Glow Type");

        public DotNode() : base()
        {

            niCenterPosition = AddInput("Center Position", 0.0);
            niAlpha = AddInput("Alpha", 1.0);
            niColor = AddInput<LedColor>("Color", new LedColorRGB(0, 0, 0));
            niGlow = AddInput("Glow", 0.0);
            niSymmetric = AddInput("Symmetric", false);

            niOutput = AddOutput<LedColor[]>("Colors");

            foreach (var s in new string[] { "Linear", "Exponential", "Gaussian" })
                noGlowType.Options.Add(new ReactiveListItem<string>(s));
            noGlowType.SelectedId.Set(noGlowType.Options[0].Id.ToString());
            Options.Add(noGlowType);

            Calculate();

        }

        public override bool Calculate()
        {

            int resolution = GlobalProperties.Instance.Resolution;

            double centerPosition = clamp(niCenterPosition.Value, 0, 1);
            double alpha = clamp(niAlpha.Value, 0, 1);
            double glow = Math.Max(0, niGlow.Value);
            LedColorHSV color = niColor.Value.GetColorHSV();
            bool symmetric = niSymmetric.Value;

            LedColorHSV[] buffer = new LedColorHSV[resolution];

            Func<double, double, double, double> intensity;
            switch (noGlowType.Value.Get())
            {
                case "Exponential":
                    intensity = exponentialIntensity;
                    break;
                case "Gaussian":
                    intensity = gaussianIntensity;
                    break;
                default:
                case "Linear":
                    intensity = linearIntensity;
                    break;
            }

            for (int i = 0; i < resolution; i++)
            {
                double position = 1.0 * i / resolution;
                buffer[i] = new LedColorHSV(color.H, color.S,
                    alpha * clamp(intensity(centerPosition, position, glow), 0, 1) * color.V);
            }

            if (symmetric)
            {
                LedColorHSV[] reverseColors = new LedColorHSV[resolution];
                for (int i = 0; i < resolution; i++)
                {
                    reverseColors[resolution - i - 1] = buffer[i];
                }
                for (int i = 0; i < resolution; i++)
                {
                    buffer[i] = buffer[i].Add(reverseColors[i], 0.5).GetColorHSV();
                }
            }

            niOutput.SetValue(buffer);

            return true;

        }

        private double clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));

        private double linearIntensity(double center, double position, double width)
        {
            if (width == 0.0) return 0.0;
            double distance = Math.Abs(position - center);
            return 1.0 - distance / width;
        }

        private double exponentialIntensity(double center, double position, double pBase)
        {
            double distance = Math.Abs(position - center);
            return Math.Pow(pBase, distance);
        }

        private double gaussianIntensity(double center, double position, double stdDeviation)
        {
            return Math.Exp(-((position - center) * (position - center)) / (2 * stdDeviation * stdDeviation)) / (stdDeviation * Math.Sqrt(2 * Math.PI));
        }

    }
}
