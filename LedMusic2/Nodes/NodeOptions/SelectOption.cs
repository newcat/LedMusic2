using LedMusic2.Reactive;

namespace LedMusic2.Nodes.NodeOptions
{
    public class SelectOption : BaseOption
    {

        public ReactiveCollection<ReactiveListItem<string>> Options { get; }
            = new ReactiveCollection<ReactiveListItem<string>>();

        public ReactivePrimitive<string> Value { get; }
            = new ReactivePrimitive<string>();

        public SelectOption(string name) : base(name, NodeOptionType.SELECT) { }

    }
}
