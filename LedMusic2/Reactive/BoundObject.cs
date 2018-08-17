using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Reactive
{
    public class BoundObject : IReactive, IDisposable
    {

        public string __Type => target.__Type;
        private ReactiveObject target;

        internal BoundObject(ReactiveObject target)
        {
            this.target = target;
        }

        public StateUpdateCollection GetFullState() => target.GetFullState();
        public StateUpdateCollection GetStateUpdates() => target.GetStateUpdates();
        public void HandleCommand(string command, JToken payload) => target.HandleCommand(command, payload);

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    target.Unbind(this);
                    target = null;
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
