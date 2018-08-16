using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace LedMusic2.Reactive
{
    public abstract class ReactiveObject : IReactive
    {

        public virtual string __Type { get; }
        private readonly Dictionary<string, IReactive> children = new Dictionary<string, IReactive>();
        private readonly Dictionary<string, Action<JToken>> commandHandlers = new Dictionary<string, Action<JToken>>();
        public ReactivePrimitive<string> VisualState { get; } = new ReactivePrimitive<string>();

        protected virtual void Initialize() { }

        public ReactiveObject()
        {
            UpdateReactiveChildren();
            __Type = GetType().ToString();
            RegisterCommand("setVisualState", (p) => VisualState.Set(p.Value<string>()));
        }

        public void RegisterCommand(string command, Action<JToken> handler)
        {
            commandHandlers.Add(command, handler);
        }

        public void UnregisterCommand(string command)
        {
            commandHandlers.Remove(command);
        }

        public StateUpdateCollection GetStateUpdates()
        {
            var updates = new StateUpdateCollection();
            foreach (var child in children)
            {
                var cupdates = child.Value?.GetStateUpdates();
                if (cupdates != null)
                    updates.Add(new StateUpdate<StateUpdateCollection>(child.Key, cupdates));
            }
            return updates.Count > 0 ? updates : null;
        }

        public StateUpdateCollection GetFullState()
        {
            var updates = new StateUpdateCollection
            {
                new StateUpdate<string>("__Type", __Type)
            };
            foreach (var child in children)
            {
                updates.Add(new StateUpdate<StateUpdateCollection>(child.Key, child.Value?.GetFullState()));
            }
            return updates;
        }

        public void HandleCommand(string command, JToken payload)
        {
            if (command.Contains("."))
            {
                // This is a command for a child, so propagate to the appropriate child
                var parts = command.Split('.');
                var childName = parts[0];
                var remainder = string.Join(".", parts.Skip(1));
                foreach (var child in children)
                {
                    if (child.Key == childName)
                    {
                        child.Value?.HandleCommand(remainder, payload);
                        return;
                    }
                }
                throw new KeyNotFoundException($"Cannot find children with name {childName} in reactive object");
            } else
            {
                if (commandHandlers.ContainsKey(command))
                    commandHandlers[command](payload);
                else
                    throw new KeyNotFoundException($"Cannot find command handler for command {command}");
            }
        }

        protected void UpdateReactiveChildren()
        {
            children.Clear();
            var props = GetType()
                .GetProperties()
                .Where(p =>
                    typeof(IReactive).IsAssignableFrom(p.PropertyType) &&
                    Attribute.GetCustomAttribute(p, typeof(ReactiveIgnoreAttribute)) == null &&
                    !p.GetAccessors().Any(a => a.IsStatic)
                );
            foreach (var p in props)
                children.Add(p.Name, (IReactive)p.GetValue(this));
        }

        public static T FromJson<T>(JToken j) where T : ReactiveObject
        {

            T obj = Activator.CreateInstance<T>();

            var state = (JObject)j;
            foreach (var prop in state.Properties())
            {

                if (!obj.children.ContainsKey(prop.Name) || prop.Value.Type == JTokenType.Null)
                    continue;

                var reflectedProperty = obj.GetType().GetProperty(prop.Name);
                if (reflectedProperty == null || reflectedProperty.GetCustomAttribute<IgnoreOnLoadAttribute>() != null)
                    continue;

                var reactiveProperty = obj.children[prop.Name];

                var value = (JObject)prop.Value;
                if (value.ContainsKey("__IsPrimitive") && (bool)value["__IsPrimitive"])
                {

                    var primitiveValue = value["Value"];

                    if (primitiveValue.Type == JTokenType.String)
                    {
                        var primitiveType = Type.GetType((string)value["__Type"]);

                        if (typeof(ISerializable).IsAssignableFrom(primitiveType))
                        {
                            ISerializable x = (ISerializable)Activator.CreateInstance(primitiveType);
                            x.Deserialize((string)primitiveValue);
                        }
                        else
                        {
                            var parseMethod = primitiveType.GetMethod("Parse");
                            if (parseMethod != null)
                                ((ReactivePrimitive)reactiveProperty).Set(parseMethod.Invoke(null, new object[] { (string)primitiveValue }));
                            else
                                ((ReactivePrimitive)reactiveProperty).Set(primitiveValue);
                        }
                    } else
                    {
                        ((ReactivePrimitive)reactiveProperty).Set(primitiveValue);
                    }

                }
                else if (isSubclassOfRawGeneric(typeof(ReactiveCollection<>), obj.children[prop.Name].GetType()))
                {
                    ((IReactiveCollection)reactiveProperty).LoadFromJson(value);
                } else
                {
                    var type = Type.GetType((string)value["__Type"]);
                    var jsonToObjectMethod = typeof(ReactiveObject).GetMethod("FromJson");
                    var m = jsonToObjectMethod.MakeGenericMethod(type);
                    var instance = m.Invoke(null, new object[] { value });
                    obj.GetType().InvokeMember(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, obj, new object[] { instance });
                }

            }

            obj.UpdateReactiveChildren();
            obj.Initialize();

            return obj;

        }

        private static ReactivePrimitive<T> getTypedProperty<T>(IReactive actual)
        {
            if (typeof(ReactivePrimitive<T>) != actual.GetType())
                throw new ArgumentException($"Expected type {typeof(ReactivePrimitive<T>)}, got {actual}.");

            return (ReactivePrimitive<T>)actual;
        }

        private static bool isSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

    }
}
