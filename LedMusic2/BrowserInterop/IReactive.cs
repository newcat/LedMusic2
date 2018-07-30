namespace LedMusic2.BrowserInterop
{
    public interface IReactive
    {
        string ReactiveName { get; }
        IStateUpdate GetStateUpdates();
        IStateUpdate GetFullState();
    }
}
