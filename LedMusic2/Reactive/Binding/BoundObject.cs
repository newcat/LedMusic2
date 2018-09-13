using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Reactive.Binding
{
    public class BoundObject : IBound, IDisposable
    {

        public string __Type => Target.__Type;
        public ReactiveObject Target { get; }

        public BoundObject(ReactiveObject target)
        {
            Target = target;
        }

        public StateUpdateCollection GetFullState(Guid requestId) => Target.GetFullState(requestId);
        public StateUpdateCollection GetStateUpdates(Guid requestId) => Target.GetStateUpdates(requestId);
        public void HandleCommand(string command, JToken payload) => Target.HandleCommand(command, payload);

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
