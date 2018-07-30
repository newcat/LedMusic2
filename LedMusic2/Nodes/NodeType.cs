using LedMusic2.BrowserInterop;
using System;

namespace LedMusic2.Nodes
{
    public class NodeType : ReactiveObject, IReactiveListItem
    {

        public override string ReactiveName => "NodeType";
        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>("Name");
        public ReactivePrimitive<NodeCategory> Category { get; } = new ReactivePrimitive<NodeCategory>("Category");

        public Type T { get; set; }

        public NodeType(string name, NodeCategory category, Type t)
        {
            Name.Set(name);
            Category.Set(category);
            T = t;
        }

    }
}
