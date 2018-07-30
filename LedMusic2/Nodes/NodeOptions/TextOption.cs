using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.Nodes.NodeOptions
{
    class TextOption : BaseOption
    {

        public override NodeOptionType Type => throw new NotImplementedException();
        public override string ReactiveName => throw new NotImplementedException();

        public ReactivePrimitive<string> Value { get; } = new ReactivePrimitive<string>("Value");

        public TextOption(string name) : base(name) { }

    }
}
