using LedMusic2.Reactive.Binding;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.Reactive
{

    public class ReactiveCollection<T> : List<T>, IReactiveCollection<T>, IBindable<BoundCollection<T>>
        where T : IReactive, IReactiveListItem
    {

        public string __Type { get; }

        public Action<string, JToken, ReactiveCollection<T>> CommandHandler { get; set; }

        private readonly List<T> addedItems = new List<T>();
        private readonly List<T> removedItems = new List<T>();

        private readonly BindHelper<BoundCollection<T>> bindHelper;

        public ReactiveCollection()
        {
            __Type = GetType().ToString();
            bindHelper = new BindHelper<BoundCollection<T>>(() => new BoundCollection<T>(this));
        }

        public new void Add(T item)
        {
            base.Add(item);
            addOperation(item);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                Add(item);
        }

        public new void Clear()
        {
            ForEach(item => removeOperation(item));
            base.Clear();
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            removeOperation(item);
        }

        public T FindById(string id)
        {
            if (id == null) return default(T);
            return FindById(Guid.Parse(id));
        }

        public T FindById(Guid id)
        {
            return Find((x) => x.Id == id);
        }

        public StateUpdateCollection GetFullState(Guid requestId)
        {
            return bindHelper.GetState(requestId, () =>
            {
                var updates = new StateUpdateCollection(new StateUpdate<string>("__Type", __Type));
                updates.AddRange((this as IEnumerable<T>)
                    .Select(item => new StateUpdate<StateUpdateCollection>(item.Id.ToString(), item.GetFullState(requestId))));
                addedItems.Clear();
                removedItems.Clear();
                return updates;
            });
        }

        public StateUpdateCollection GetStateUpdates(Guid requestId)
        {
            return bindHelper.GetState(requestId, () =>
            {
                var updates = new StateUpdateCollection();
                updates.AddRange(addedItems.Select(item => new StateUpdate<StateUpdateCollection>(item.Id.ToString(), item.GetFullState(requestId))));
                updates.AddRange(removedItems.Select(item => new StateUpdate<string>(item.Id.ToString(), "__Deleted")));                
                foreach (var item in this.Except(addedItems).Except(removedItems))
                {
                    var itemUpdates = item.GetStateUpdates(requestId);
                    if (itemUpdates != null)
                        updates.Add(new StateUpdate<StateUpdateCollection>(item.Id.ToString(), itemUpdates));
                }
                addedItems.Clear();
                removedItems.Clear();
                return updates.Count > 0 ? updates : null;
            });
        }

        public void HandleCommand(string command, JToken payload)
        {
            if (command.Contains("."))
            {
                // This is a command for a child, so propagate to the appropriate child
                var parts = command.Split('.');
                var childName = parts[0];
                var remainder = string.Join(".", parts.Skip(1));
                var child = FindById(childName);
                if (child != null)
                    child.HandleCommand(remainder, payload);
                else
                    throw new KeyNotFoundException($"Cannot find element with name {childName} in reactive collection");
            } else
            {
                CommandHandler?.Invoke(command, payload, this);
            }
        }

        public void LoadFromJson(JToken j)
        {

            Clear();

            foreach (var item in (JObject)j)
            {

                if (item.Key == "__Type")
                    continue;

                var id = Guid.Parse(item.Key);
                var itemObject = (JObject)item.Value;

                var type = Type.GetType((string)itemObject["__Type"]);

                if (type == null)
                    throw new TypeLoadException($"Could not find type '{(string)itemObject["__Type"]}'");

                var instance = (T)Activator.CreateInstance(type, itemObject);
                instance.Id = Guid.Parse(item.Key);
                Add(instance);

            }

        }

        public BoundCollection<T> Bind() => bindHelper.Bind();
        public void Unbind(BoundCollection<T> boundObject) => bindHelper.Unbind(boundObject);

        private void addOperation(T item)
        {
            addedItems.Add(item);
        }

        private void removeOperation(T item)
        {
            removedItems.Add(item);
        }

    }

}
