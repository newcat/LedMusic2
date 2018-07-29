using System;

namespace LedMusic2.BrowserInterop
{
    public class SendMessageEventArgs : EventArgs
    {
        public string Command { get; }
        public byte[] Payload { get; }
        
        public SendMessageEventArgs(string command, byte[] payload)
        {
            Command = command;
            Payload = payload;
        }
    }
}
