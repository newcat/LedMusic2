using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System;

namespace LedMusic2.Outputs
{
    public class OutputType : ReactiveObject, ICombinedReactive
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>();
        public Type T { get; set; }

        public OutputType() { }

        public OutputType(string name, Type t)
        {
            Name.Set(name);
            T = t;
        }

        public OutputType(JToken j)
        {
            throw new NotImplementedException();
        }

    }
}
