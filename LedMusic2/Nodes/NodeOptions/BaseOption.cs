using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Nodes.NodeOptions
{
    public abstract class BaseOption : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>("");
        public ReactivePrimitive<NodeOptionType> Type { get; } = new ReactivePrimitive<NodeOptionType>(NodeOptionType.BOOL);

        public event EventHandler ValueChanged;

        private BaseOption()
        {
            RegisterCommand("setValue", (p) => {
                SetValue(p);
                RaiseValueChanged();
            });
        }

        public BaseOption(JToken j) : this()
        {
            LoadState(j);
        }

        public BaseOption(string name, NodeOptionType type) : this()
        {
            Name.Set(name);
            Type.Set(type);
        }

        public abstract object GetValue();
        protected abstract void SetValue(JToken value);

        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
        }

    }
}
