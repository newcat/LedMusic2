using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.Reactive
{
    public abstract class ReactiveObject : IReactive
    {

        public virtual string __Type { get; }
        private readonly Dictionary<string, IReactive> children = new Dictionary<string, IReactive>();
        private readonly Dictionary<string, Action<object>> commandHandlers = new Dictionary<string, Action<object>>();

        public ReactiveObject()
        {
            var props = GetType()
                .GetProperties()
                .Where(p =>
                    typeof(IReactive).IsAssignableFrom(p.PropertyType) &&
                    Attribute.GetCustomAttribute(p, typeof(ReactiveIgnoreAttribute)) == null &&
                    !p.GetAccessors().Any(a => a.IsStatic)
                );
            foreach (var p in props)
            {
                children.Add(p.Name, (IReactive)p.GetValue(this));
            }
            __Type = GetType().ToString();
        }

        public void RegisterCommand(string command, Action<object> handler)
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

        public void HandleCommand(string command, object payload)
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

    }
}
