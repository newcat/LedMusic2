using System;

namespace LedMusic2.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    class OutputAttribute : Attribute
    {

        public string Name { get; private set; }

        public OutputAttribute(string name)
        {
            Name = name;
        }

    }
}
