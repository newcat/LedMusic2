using LedMusic2.BrowserInterop;

namespace LedMusic2.Nodes.NodeOptions
{
    public class BoolOption : BaseOption
    {

        public override NodeOptionType Type => NodeOptionType.BOOL;
        public override string ReactiveName => "BoolOption";

        public ReactivePrimitive<bool> Value = new ReactivePrimitive<bool>("Value", false);

        public BoolOption(string name) : base(name) { }

    }
}
