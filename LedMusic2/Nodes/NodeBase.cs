using LedMusic2.Reactive;
using LedMusic2.LedColors;
using LedMusic2.NodeConnection;
using LedMusic2.Nodes.NodeOptions;
using System;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{
    public abstract class NodeBase : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual string NodeName
        {
            get
            {
                var attr = (NodeAttribute[])GetType().GetCustomAttributes(typeof(NodeAttribute), true);
                if (attr.Length > 0)
                    return attr[0].Name;
                else
                    return "Node";
            }
        }

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>();
        public NodeInterfaceList Inputs { get; } = new NodeInterfaceList();
        public NodeInterfaceList Outputs { get; } = new NodeInterfaceList();
        public ReactiveCollection<BaseOption> Options { get; } = new ReactiveCollection<BaseOption>();

        protected NodeBase()
        {
            Name.Set(NodeName);
        }

        public abstract bool Calculate();

        protected NodeInterface<T> AddInput<T>(string name)
        {
            var ni = new NodeInterface<T>(name, inferConnectionType<T>(), this, true);
            Inputs.Add(ni);
            return ni;
        }
        protected NodeInterface<T> AddInput<T>(string name, T initialValue)
        {
            var ni = new NodeInterface<T>(name, inferConnectionType<T>(), this, true, initialValue);
            Inputs.Add(ni);
            return ni;
        }

        protected NodeInterface<T> AddOutput<T>(string name)
        {
            var ni = new NodeInterface<T>(name, inferConnectionType<T>(), this, false);
            Outputs.Add(ni);
            return ni;
        }

        private ConnectionType inferConnectionType<T>()
        {

            if (typeof(T) == typeof(double))
                return ConnectionType.NUMBER;
            else if (typeof(T) == typeof(LedColor))
                return ConnectionType.COLOR;
            else if (typeof(T) == typeof(LedColorArray))
                return ConnectionType.COLOR_ARRAY;
            else if (typeof(T) == typeof(bool))
                return ConnectionType.BOOL;
            else
                throw new ArgumentException("Invalid type: " + typeof(T).ToString());

        }

    }
}
