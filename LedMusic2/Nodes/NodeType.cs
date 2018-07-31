using LedMusic2.Reactive;
using System;

namespace LedMusic2.Nodes
{
    public class NodeType : ReactiveObject, IReactiveListItem
    {
        
        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>();
        public ReactivePrimitive<NodeCategory> Category { get; } = new ReactivePrimitive<NodeCategory>();

        public Type T { get; set; }

        public NodeType(string name, NodeCategory category, Type t)
        {
            Name.Set(name);
            Category.Set(category);
            T = t;
        }

    }
}
