using AttachedCommandBehavior;
using LedMusic2.Enums;
using LedMusic2.Nodes;
using LedMusic2.ViewModels;
using LedMusic2.Views;
using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace LedMusic2.Models
{

    public abstract class NodeInterface : VMBase
    {

        public string Name { get; private set; }
        public ConnectionType ConnectionType { get; private set; }
        public abstract Type NodeType { get; }
        public NodeBase Parent { get; private set; }

        #region ViewModel Properties
        private NodeInterfaceView _view = null;
        public NodeInterfaceView View
        {
            get { return _view; }
            set
            {
                _view = value;
                NotifyPropertyChanged();
            }
        }

        private NodeOptionViewModel _option = null;
        public NodeOptionViewModel Option
        {
            get { return _option; }
            set
            {
                _option = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isInput = false;
        public bool IsInput
        {
            get { return _isInput; }
            set
            {
                _isInput = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Commands
        private SimpleCommand _cmdEllipseClicked = new SimpleCommand();
        public SimpleCommand CmdEllipseClicked
        {
            get { return _cmdEllipseClicked; }
            set
            {
                _cmdEllipseClicked = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public event EventHandler ValueChanged;

        private Guid _id = Guid.NewGuid();
        public Guid Id { get { return _id; } set { _id = value; } }

        public NodeInterface(string name, ConnectionType ctype, NodeBase parent, bool isInput)
        {
            Name = name;
            ConnectionType = ctype;
            Parent = parent;
            IsInput = isInput;
            _cmdEllipseClicked.ExecuteDelegate = (o) => MainViewModel.Instance.CreateTemporaryConnection(this);

            if (isInput)
            {
                switch (ctype)
                {
                    case ConnectionType.BOOL:
                        Option = new NodeOptionViewModel(NodeOptionType.BOOL, name);
                        Option.PropertyChanged += Option_PropertyChanged;
                        break;
                    case ConnectionType.COLOR:
                        Option = new NodeOptionViewModel(NodeOptionType.COLOR, name);
                        Option.PropertyChanged += Option_PropertyChanged;
                        break;
                    case ConnectionType.NUMBER:
                        Option = new NodeOptionViewModel(NodeOptionType.NUMBER, name);
                        Option.PropertyChanged += Option_PropertyChanged;
                        break;
                }
            }

        }

        public XElement GetXmlElement()
        {
            XElement interfaceX = new XElement("nodeinterface");
            interfaceX.SetAttributeValue("name", Name);
            interfaceX.SetAttributeValue("id", Id);
            if (Option != null)
                interfaceX.Add(Option.GetXmlElement());
            return interfaceX;
        }

        public void LoadFromXml(XElement niX)
        {
            Id = Guid.Parse(niX.Attribute("id").Value);
            foreach (XElement el in niX.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "nodeoption":
                        Option.LoadFromXml(el);
                        break;
                }
            }
        }

        private void Option_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsConnected && e.PropertyName == "RenderValue")
            {
                SetValue(Option.RenderValue);
                Parent.InvokeOutputChanged();
            }
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
            if (Option != null && IsInput && !IsConnected)
            {
                Option.DisplayValue = GetValue();
            }
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
