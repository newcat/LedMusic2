using System;

namespace LedMusic2.BrowserInterop
{

    public abstract class ReactivePrimitive : IReactive
    {

        public string ReactiveName { get; }

        public ReactivePrimitive(string name)
        {
            ReactiveName = name;
        }

        public abstract IStateUpdate GetStateUpdates();
        public abstract IStateUpdate GetFullState();

    }

    public class ReactivePrimitive<T> : ReactivePrimitive
    {

        private T value;
        private StateUpdate<T> stateUpdate;

        public Func<T, T> CustomGetter { get; set; }
        public Func<T, T> CustomSetter { get; set; }

        public ReactivePrimitive(string name) : base(name) { }

        public ReactivePrimitive(string name, T initialValue) : this(name)
        {
            Set(initialValue);
        }

        public T Get()
        {
            if (CustomGetter != null)
                return CustomGetter(value);
            else
                return value;
        }

        public void Set(T newValue)
        {

            if (CustomSetter != null)
                newValue = CustomSetter(newValue);

            if (!newValue.Equals(value))
            {
                value = newValue;
                stateUpdate = new StateUpdate<T>(ReactiveName, value);
            }

        }

        public override IStateUpdate GetStateUpdates()
        {
            return stateUpdate;
        }

        public override IStateUpdate GetFullState()
        {
            return new StateUpdate<T>(ReactiveName, value);
        }

    }
}
