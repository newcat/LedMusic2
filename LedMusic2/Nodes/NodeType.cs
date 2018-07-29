using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.Nodes
{
    public class NodeType : ISimpleSynchronizable
    {

        public string Name { get; set; }
        public NodeCategory Category { get; set; }
        public Type T { get; set; }

        public Guid Id { get; } = Guid.NewGuid();

        public NodeType(string name, NodeCategory category, Type t)
        {
            Name = name;
            Category = category;
            T = t;
        }

    }
}
