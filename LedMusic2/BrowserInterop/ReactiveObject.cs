using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LedMusic2.BrowserInterop
{
    public abstract class ReactiveObject : IReactive
    {

        public abstract string ReactiveName { get; }
        private readonly List<IReactive> children = new List<IReactive>();

        public ReactiveObject()
        {
            var props = GetType().GetProperties();
            children.AddRange(props
                .Where(p => typeof(IReactive).IsAssignableFrom(p.PropertyType) && !p.GetAccessors().Any(a => a.IsStatic))
                .Select(p => (IReactive)p.GetValue(this))
            );
        }

        public IStateUpdate GetStateUpdates()
        {
            return new StateUpdate<StateUpdateCollection>(ReactiveName, new StateUpdateCollection(children.Select(c => c?.GetStateUpdates())));
        }

        public IStateUpdate GetFullState()
        {
            return new StateUpdate<StateUpdateCollection>(ReactiveName, new StateUpdateCollection(children.Select(c => c?.GetFullState())));
        }

    }
}
