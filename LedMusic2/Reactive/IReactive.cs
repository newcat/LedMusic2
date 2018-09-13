using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Reactive
{
    public interface IReactive
    {
        string __Type { get; }
        StateUpdateCollection GetStateUpdates(Guid requestId);
        StateUpdateCollection GetFullState(Guid requestId);
        void HandleCommand(string command, JToken payload);
    }
}
