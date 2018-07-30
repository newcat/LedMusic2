using LedMusic2.LedColors;
using System;

namespace LedMusic2.Nodes.NodeModels
{
    public class ColorStopViewModel : IComparable<ColorStopViewModel>
    {

        public double Position { get; set; }
        public LedColorRGB Color { get; set; }

        public ColorStopViewModel(LedColor c, double p)
        {
            Color = c.GetColorRGB();
            Position = p;
        }

        public int CompareTo(ColorStopViewModel other) => Position.CompareTo(other.Position);

    }
}
