using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LedMusic2.LedColors;
using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class ColorRampOption : BaseOption
    {

        public ReactiveCollection<ColorStop> ColorStops = new ReactiveCollection<ColorStop>();

        public ColorRampOption() : base("Color Ramp", NodeOptionType.COLORRAMP) { }

        public override object GetValue() => throw new NotImplementedException();
        protected override void SetValue(JToken value) => throw new NotImplementedException();

    }

    public class ColorStop : ReactiveObject, IComparable<ColorStop>, IReactiveListItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public ReactivePrimitive<double> Position { get; } = new ReactivePrimitive<double>(0.5);
        public ReactivePrimitive<LedColor> Color { get; } = new ReactivePrimitive<LedColor>(new LedColor(0, 0, 0));

        public ColorStop() { }
        public ColorStop(LedColor color, double position) { Color.Set(color); Position.Set(position); }
        public ColorStop(JToken j) { LoadState(j); }
        public int CompareTo(ColorStop other) => Position.Get().CompareTo(other.Position.Get());
    }

}
