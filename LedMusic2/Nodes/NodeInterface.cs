﻿using LedMusic2.Reactive;
using LedMusic2.NodeConnection;
using System;
using System.Xml.Linq;
using LedMusic2.Nodes.NodeOptions;

namespace LedMusic2.Nodes
{

    public abstract class NodeInterface : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public abstract Type NodeType { get; }
        [ReactiveIgnore]
        public NodeBase Parent { get; private set; }

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>();
        public ReactivePrimitive<ConnectionType> ConnectionType { get; } = new ReactivePrimitive<ConnectionType>();
        public ReactivePrimitive<bool> IsInput { get; } = new ReactivePrimitive<bool>(false);
        public ReactivePrimitive<bool> IsConnected { get; } = new ReactivePrimitive<bool>(false);
        public BaseOption Option { get; private set; }

        public event EventHandler ValueChanged;

        public NodeInterface() { }

        public NodeInterface(string name, ConnectionType ctype, NodeBase parent, bool isInput)
        {
            Parent = parent;
            Name.Set(name);
            ConnectionType.Set(ctype);
            IsInput.Set(isInput);
        }

        protected override void Initialize()
        {
            if (IsInput.Get())
            {
                switch (ConnectionType.Get())
                {
                    case NodeConnection.ConnectionType.BOOL:
                        Option = new BoolOption(Name.Get());
                        break;
                    case NodeConnection.ConnectionType.COLOR:
                        Option = new ColorOption(Name.Get());
                        break;
                    case NodeConnection.ConnectionType.NUMBER:
                        Option = new NumberOption(Name.Get());
                        break;
                }
                if (Option != null)
                {
                    Option.ValueChanged += option_ValueChanged;
                    UpdateReactiveChildren();
                }
            }
        }

        private void option_ValueChanged(object sender, EventArgs e)
        {
            SetValue(Option.GetValue());
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

        public NodeInterface() : base() { }

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
