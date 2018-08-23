using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LedMusic2.Reactive.Binding
{
    public class BindHelper<TBound>
    {

        private readonly List<TBound> boundObjects = new List<TBound>();
        private StateUpdateCollection cachedUpdates = null;
        private int sentCounter = 0;
        private readonly object sync = new object();
        private readonly object stateUpdateInProgress = new object();

        private readonly Func<TBound> createBoundObject;

        public BindHelper(Func<TBound> createBoundObject)
        {
            this.createBoundObject = createBoundObject;
        }

        public TBound Bind()
        {
            lock (sync)
            lock (stateUpdateInProgress)
            {
                var b = createBoundObject();
                boundObjects.Add(b);
                return b;
            }
        }

        public void Unbind(TBound b)
        {
            lock (sync)
            lock (stateUpdateInProgress)
            {
                boundObjects.Remove(b);
            }
        }

        public StateUpdateCollection GetState(Func<StateUpdateCollection> stateCreator)
        {
            lock (sync)
            {
                if (sentCounter == 0)
                {
                    cachedUpdates = stateCreator();
                    Monitor.Enter(stateUpdateInProgress);
                }

                if (++sentCounter > boundObjects.Count)
                {
                    sentCounter = 0;
                    Monitor.Exit(stateUpdateInProgress);
                }

                return cachedUpdates;
            }
        }

    }
}
