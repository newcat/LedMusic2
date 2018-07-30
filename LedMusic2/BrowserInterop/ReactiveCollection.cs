using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.BrowserInterop
{

    public class ReactiveCollection<T> : List<T>, IReactive
        where T : ReactiveObject, IReactiveListItem
    {

        public string ReactiveName { get; }
        public bool ChildIsReactive { get; } = true;
        private StateUpdateCollection stateUpdates = new StateUpdateCollection();

        public ReactiveCollection(string name)
        {
            ReactiveName = name;
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

        public IStateUpdate GetFullState()
        {
            return new StateUpdate<StateUpdateCollection>(ReactiveName,
                new StateUpdateCollection((this as IEnumerable<T>).Select(item => item.GetFullState()))
            );
        }

        public IStateUpdate GetStateUpdates()
        {
            var updates = stateUpdates;
            stateUpdates = new StateUpdateCollection();
            return new StateUpdate<StateUpdateCollection>(ReactiveName, updates);
        }

        private void addOperation(T item)
        {
            stateUpdates.Add(new StateUpdate<IStateUpdate>(item.Id.ToString(), item.GetFullState()));
        }

        private void removeOperation(T item)
        {
            stateUpdates.Add(new StateUpdate<string>(item.Id.ToString(), "$DELETED"));
        }

    }

}
