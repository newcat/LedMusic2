using System;

namespace LedMusic2.Nodes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    class NodeAttribute : Attribute
    {

        public string Name { get; private set; }
        public NodeCategory Category { get; private set; }

        public NodeAttribute(string name, NodeCategory category)
        {

            Name = name;
            Category = category;

        }

    }
}
