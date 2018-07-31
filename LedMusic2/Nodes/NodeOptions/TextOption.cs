using LedMusic2.Reactive;
using System;

namespace LedMusic2.Nodes.NodeOptions
{
    class TextOption : BaseOption
    {

        public override NodeOptionType Type => NodeOptionType.TEXT;

        public ReactivePrimitive<string> Value { get; } = new ReactivePrimitive<string>();

        public TextOption(string name) : base(name) { }

    }
}
