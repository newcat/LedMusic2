using LedMusic2.Reactive;
using LedMusic2.LedColors;

namespace LedMusic2.Nodes.NodeOptions
{
    public class ColorOption : BaseOption
    {

        public ReactivePrimitive<LedColor> Value = new ReactivePrimitive<LedColor>(new LedColorRGB(0, 0, 0));

        public ColorOption(string name) : base(name, NodeOptionType.COLOR) { }

    }
}
