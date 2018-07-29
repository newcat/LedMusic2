using System;

namespace LedMusic2.BrowserInterop
{

    public interface ISimpleSynchronizable
    {
        Guid Id { get; }
    }

    public interface ISynchronizable : ISimpleSynchronizable
    {
        event EventHandler<SendMessageEventArgs> SendMessage;
        void ReceiveMessage(string command, byte[] payload);
    }

}
