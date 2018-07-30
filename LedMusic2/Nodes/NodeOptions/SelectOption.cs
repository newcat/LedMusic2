using LedMusic2.BrowserInterop;

namespace LedMusic2.Nodes.NodeOptions
{
    public class SelectOption : BaseOption
    {

        public override NodeOptionType Type => NodeOptionType.SELECT;
        public override string ReactiveName => "SelectOption";

        public ReactiveCollection<ReactiveListItem<string>> Options { get; }
            = new ReactiveCollection<ReactiveListItem<string>>("Options");

        public ReactivePrimitive<string> Value { get; }
            = new ReactivePrimitive<string>("Value");

        public SelectOption(string name) : base(name) { }

    }
}
