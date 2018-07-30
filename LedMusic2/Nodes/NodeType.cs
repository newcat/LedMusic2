using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.Nodes
{
    public class NodeType : ReactiveObject, IReactiveListItem
    {

        public override string ReactiveName => "NodeType";
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }
        public NodeCategory Category { get; set; }
        public Type T { get; set; }


        public NodeType(string name, NodeCategory category, Type t)
        {
            Name = name;
            Category = category;
            T = t;
        }

    }
}
