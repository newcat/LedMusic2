using LedMusic2.Reactive;

namespace LedMusic2.Nodes.NodeOptions
{
    class TextOption : BaseOption
    {

        public ReactivePrimitive<string> Value { get; } = new ReactivePrimitive<string>();

        public TextOption(string name) : base(name, NodeOptionType.TEXT) { }

    }
}
