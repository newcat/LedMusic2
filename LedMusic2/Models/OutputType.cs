using System;

namespace LedMusic2.Models
{
    public class OutputType
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
