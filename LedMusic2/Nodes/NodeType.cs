using System;

namespace LedMusic2.Nodes
{
    public class NodeType
    {

        public string Name { get; set; }
        public Type T { get; set; }

        public NodeType(string name, Type t)
        {
            Name = name;
            T = t;
        }

    }
}
