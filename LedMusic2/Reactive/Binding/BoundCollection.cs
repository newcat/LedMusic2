using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LedMusic2.Reactive.Binding
{
    public class BoundCollection<T> : IReactiveCollection<T>, IBound, IDisposable
        where T : IReactive, IReactiveListItem
    {

        public string __Type => Target.__Type;
        public ReactiveCollection<T> Target { get; }

        public BoundCollection(ReactiveCollection<T> target)
        {
            Target = target;
        }

        public StateUpdateCollection GetFullState() => Target.GetFullState();
        public StateUpdateCollection GetStateUpdates() => Target.GetStateUpdates();
        public void HandleCommand(string command, JToken payload) => Target.HandleCommand(command, payload);
        public T FindById(string id) => Target.FindById(id);
        public T FindById(Guid id) => Target.FindById(id);
        public IEnumerator<T> GetEnumerator() => Target.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Target.GetEnumerator();

        public void LoadFromJson(JToken j)
        {
            // Loading wont work here, just ignore
            // Target should be loaded anyway
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Target.Unbind(this);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion 

    }
}
