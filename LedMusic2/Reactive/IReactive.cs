﻿namespace LedMusic2.Reactive
{
    public interface IReactive
    {
        string __Type { get; }
        StateUpdateCollection GetStateUpdates();
        StateUpdateCollection GetFullState();
        void HandleCommand(string command, object payload);
    }
}
