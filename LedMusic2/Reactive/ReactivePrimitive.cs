using System;

namespace LedMusic2.Reactive
{

    public abstract class ReactivePrimitive : IReactive
    {

        public abstract string __Type { get; }

        public abstract StateUpdateCollection GetStateUpdates();
        public abstract StateUpdateCollection GetFullState();
        public abstract void HandleCommand(string command, object payload);

    }

    public class ReactivePrimitive<T> : ReactivePrimitive
    {

        private T value;
        private StateUpdate<T> stateUpdate;

        public override string __Type => typeof(T).ToString();
        public Func<T, T> CustomGetter { get; set; }
        public Func<T, T> CustomSetter { get; set; }

        public ReactivePrimitive() { }
        public ReactivePrimitive(T initialValue)
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
                stateUpdate = new StateUpdate<T>("Value", value);
            }

        }

        public override StateUpdateCollection GetStateUpdates()
        {
            return stateUpdate != null ? new StateUpdateCollection(stateUpdate) : null;
        }

        public override StateUpdateCollection GetFullState()
        {
            return new StateUpdateCollection(
                new StateUpdate<string>("__Type", __Type),
                new StateUpdate<T>("Value", Get())
            );
        }

        public override void HandleCommand(string command, object payload)
        {
            if (command != "set")
                throw new InvalidOperationException($"Command '{command}' is not supported by ReactiveProperty");
            else if (payload.GetType() != typeof(T))
                throw new ArgumentException("Type does not match type of reactive property", "payload");
            else
                Set((T)payload);
        }

    }
}
