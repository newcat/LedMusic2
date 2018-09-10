using LedMusic2.LedColors;
using System;

namespace LedMusic2.Nodes.NodeModels
{
    public class ColorStopViewModel : IComparable<ColorStopViewModel>
    {

        public double Position { get; set; }
        public LedColor Color { get; set; }

        public ColorStopViewModel(LedColor c, double p)
        {
            Color = c;
            Position = p;
        }

        public int CompareTo(ColorStopViewModel other) => Position.CompareTo(other.Position);

    }
}
