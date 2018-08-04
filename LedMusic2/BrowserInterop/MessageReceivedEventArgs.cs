using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.BrowserInterop
{
    public class MessageReceivedEventArgs : EventArgs
    {

        public JToken Message { get; }

        public MessageReceivedEventArgs(JToken message)
        {
            Message = message;
        }

    }
}
