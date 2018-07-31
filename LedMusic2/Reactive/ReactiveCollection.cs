using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.Reactive
{

    public class ReactiveCollection<T> : List<T>, IReactive
        where T : IReactive, IReactiveListItem
    {

        public string __Type { get; }

        private readonly List<T> addedItems = new List<T>();
        private readonly List<T> removedItems = new List<T>();

        public ReactiveCollection()
        {
            __Type = GetType().ToString();
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

        public StateUpdateCollection GetFullState()
        {
            var updates = new StateUpdateCollection(new StateUpdate<string>("__Type", __Type));
            updates.AddRange((this as IEnumerable<T>)
                .Select(item => new StateUpdate<StateUpdateCollection>(item.Id.ToString(), item.GetFullState())));
            addedItems.Clear();
            removedItems.Clear();
            return updates;
        }

        public StateUpdateCollection GetStateUpdates()
        {
            var updates = new StateUpdateCollection();
            updates.AddRange(addedItems.Select(item => new StateUpdate<StateUpdateCollection>(item.Id.ToString(), item.GetFullState())));
            updates.AddRange(removedItems.Select(item => new StateUpdate<string>(item.Id.ToString(), "__Deleted")));
            addedItems.Clear();
            removedItems.Clear();
            foreach (var item in this)
            {
                var itemUpdates = item.GetStateUpdates();
                if (itemUpdates != null)
                    updates.Add(new StateUpdate<StateUpdateCollection>(item.Id.ToString(), itemUpdates));
            }
            return updates.Count > 0 ? updates : null;
        }

        public void HandleCommand(string command, object payload)
        {
            // TODO
        }

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
