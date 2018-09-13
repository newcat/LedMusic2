using System;
using System.Collections.Generic;

namespace LedMusic2.Reactive.Binding
{
    public class BindHelper<TBound>
    {

        private readonly List<TBound> boundObjects = new List<TBound>();
        private StateUpdateCollection cachedUpdates = null;
        private Guid currentRequest = Guid.Empty;

        private readonly Func<TBound> createBoundObject;

        public BindHelper(Func<TBound> createBoundObject)
        {
            this.createBoundObject = createBoundObject;
        }

        public TBound Bind()
        {
            var b = createBoundObject();
            boundObjects.Add(b);
            return b;
        }

        public void Unbind(TBound b)
        {
            boundObjects.Remove(b);
        }

        public StateUpdateCollection GetState(Guid requestId, Func<StateUpdateCollection> stateCreator)
        {
            if (requestId != currentRequest)
            {
                cachedUpdates = stateCreator();
                currentRequest = requestId;
            }
            return cachedUpdates;
        }

    }
}
