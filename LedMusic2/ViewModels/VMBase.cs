using LedMusic2.BrowserInterop;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LedMusic2.ViewModels
{
    public abstract class VMBase : ISynchronizable
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<SendMessageEventArgs> SendMessage;

        public void ReceiveMessage(string command, byte[] payload)
        {
            throw new NotImplementedException();
        }

        protected void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            // TODO: Send message
        }
    }
}
