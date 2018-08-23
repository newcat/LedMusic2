using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Reactive.Binding
{
    public class BoundPrimitive : IBound, IDisposable
    {

        public string __Type => Target.__Type;
        public ReactivePrimitive Target { get; }

        public BoundPrimitive(ReactivePrimitive target)
        {
            Target = target;
        }

        public StateUpdateCollection GetFullState() => Target.GetFullState();
        public StateUpdateCollection GetStateUpdates() => Target.GetStateUpdates();
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
