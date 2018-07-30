using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.Outputs
{
    public class OutputType : ReactiveObject, IReactiveListItem
    {

        public override string ReactiveName => "OutputType";
        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>("OutputName");
        public Type T { get; set; }

        public OutputType(string name, Type t)
        {
            Name.Set(name);
            T = t;
        }

    }
}
