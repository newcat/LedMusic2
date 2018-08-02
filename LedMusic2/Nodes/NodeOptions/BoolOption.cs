using LedMusic2.Reactive;

namespace LedMusic2.Nodes.NodeOptions
{
    public class BoolOption : BaseOption
    {

        public ReactivePrimitive<bool> Value = new ReactivePrimitive<bool>(false);

        public BoolOption(string name) : base(name, NodeOptionType.BOOL) { }

    }
}
