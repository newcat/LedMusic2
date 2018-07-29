using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.Outputs
{
    public class OutputType : ISynchronizable
    {

        public string Name { get; set; }
        public Type T { get; set; }

        public OutputType(string name, Type t)
        {
            Name = name;
            T = t;
        }

    }
}
