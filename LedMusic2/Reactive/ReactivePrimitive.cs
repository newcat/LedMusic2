using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.Reactive
{

    public abstract class ReactivePrimitive : IReactive
    {

        public abstract string __Type { get; }

        public abstract StateUpdateCollection GetStateUpdates();
        public abstract StateUpdateCollection GetFullState();
        public abstract void HandleCommand(string command, JToken payload);

        public abstract void Set(object value);

    }

    public class ReactivePrimitive<T> : ReactivePrimitive
    {

        private T value;
        private IStateUpdate stateUpdate;

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

            if ((isNullOrDefault(newValue) && !isNullOrDefault(value)) ||
                (!isNullOrDefault(newValue) && isNullOrDefault(value)) ||
                (!isNullOrDefault(newValue) && !isNullOrDefault(value) && !newValue.Equals(value)))
            {
                value = newValue;
                stateUpdate = getStateUpdate();
            }

        }

        public override void Set(object value)
        {
            if (value == null)
                Set(default(T));
            else if (typeof(T).IsEnum && typeof(int).IsAssignableFrom(value.GetType()))
                Set((T)value);
            else if (!typeof(T).IsAssignableFrom(value.GetType()))
                throw new ArgumentException($"Expected type {typeof(T)}, got {value.GetType()}");
            else
                Set((T)value);
        }

        public override StateUpdateCollection GetStateUpdates()
        {
            var res = stateUpdate != null ? new StateUpdateCollection(
                new StateUpdate<string>("__Type", __Type),
                new StateUpdate<bool>("__IsPrimitive", true),
                stateUpdate
            ) : null;
            stateUpdate = null;
            return res;
        }

        public override StateUpdateCollection GetFullState()
        {
            stateUpdate = null;
            return new StateUpdateCollection(
                new StateUpdate<string>("__Type", __Type),
                new StateUpdate<bool>("__IsPrimitive", true),
                getStateUpdate()
            );
        }

        public override void HandleCommand(string command, JToken payload)
        {
            if (command != "set")
                throw new InvalidOperationException($"Command '{command}' is not supported by ReactiveProperty");
            //TODO
            //else if ((payload as JValue).Type .GetType() != typeof(T))
            //    throw new ArgumentException("Type does not match type of reactive property", "payload");
            else
            {
                if (typeof(ISerializable).IsAssignableFrom(typeof(T)))
                {
                    (Get() as ISerializable).Deserialize(payload.Value<string>());
                } else
                {
                    Set((payload as JValue).Value<T>());
                }
            }
                
        }

        private IStateUpdate getStateUpdate()
        {
            if (isNullOrDefault(Get()))
                return new StateUpdate<T>("Value", Get());

            if (typeof(ISerializable).IsAssignableFrom(typeof(T)))
            {
                return new StateUpdate<string>("Value", (Get() as ISerializable).Serialize());
            } else
            {
                return new StateUpdate<T>("Value", Get());
            }
        }

        private bool isNullOrDefault(T val)
        {
            return EqualityComparer<T>.Default.Equals(val, default(T));
        }

    }
}
