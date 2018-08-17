using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Nodes.NodeOptions
{
    public class SelectOption<T> : BaseOption
        where T : ICombinedReactive
    {

        public ReactiveCollection<T> Options { get; private set; }
            = new ReactiveCollection<T>();

        public ReactivePrimitive<string> SelectedId { get; }
            = new ReactivePrimitive<string>();

        public ReactivePrimitive<string> ItemDisplayPropertyName { get; }
            = new ReactivePrimitive<string>("");

        [ReactiveIgnore]
        public T Value
        {
            get
            {
                var item = Options.FindById(SelectedId.Get());
                return item != null ? item : default(T);
            }
        }

        public SelectOption(JToken j) : base(j) { }
        public SelectOption(string name) : base(name, NodeOptionType.SELECT) { }

        public void SetOptions(ReactiveCollection<T> newOptions)
        {
            Options = newOptions;
            UpdateReactiveChildren();
        }

        protected override void SetValue(JToken value)
        {
            SelectedId.Set((string)value);
        }

        public override object GetValue()
        {
            return Value;
        }

    }
}
