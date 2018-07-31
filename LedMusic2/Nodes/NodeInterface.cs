using LedMusic2.Reactive;
using LedMusic2.NodeConnection;
using System;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{

    public abstract class NodeInterface : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = new Guid();
        public string Name { get; private set; }
        public ConnectionType ConnectionType { get; private set; }
        public abstract Type NodeType { get; }
        public NodeBase Parent { get; private set; }

        public ReactivePrimitive<bool> IsInput { get; } = new ReactivePrimitive<bool>(false);
        public ReactivePrimitive<bool> IsConnected { get; } = new ReactivePrimitive<bool>(false);

        public event EventHandler ValueChanged;

        public NodeInterface(string name, ConnectionType ctype, NodeBase parent, bool isInput)
        {
            Name = name;
            ConnectionType = ctype;
            IsInput.Set(isInput);
        }

        public XElement GetXmlElement()
        {
            XElement interfaceX = new XElement("nodeinterface");
            interfaceX.SetAttributeValue("name", Name);
            interfaceX.SetAttributeValue("id", Id);
            return interfaceX;
        }

        public void LoadFromXml(XElement niX)
        {
            Id = Guid.Parse(niX.Attribute("id").Value);
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
        }

        public abstract bool SetValue(object input);
        public abstract object GetValue();

    }

    public class NodeInterface<T> : NodeInterface
    {

        public override Type NodeType { get { return typeof(T); } }
        public T Value { get; private set; }

        public NodeInterface(string name, ConnectionType cType, NodeBase parent, bool isInput) :
            base(name, cType, parent, isInput)
        { }

        public NodeInterface(string name, ConnectionType cType, NodeBase parent, bool isInput, T initialValue) :
            base(name, cType, parent, isInput)
        {
            SetValue(initialValue);
        }

        public override bool SetValue(object input)
        {

            if (input == null)
            {
                return false;
            }
            else if (NodeType.IsAssignableFrom(input.GetType()))
            {
                object oldValue = Value;
                Value = (T)input;

                if (oldValue == null || !oldValue.Equals(input))
                    base.OnValueChanged();

                return true;
            }
            else
            {
                return false;
            }

        }

        public override object GetValue()
        {
            return Value;
        }

    }
}
