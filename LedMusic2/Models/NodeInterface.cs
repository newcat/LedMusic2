using AttachedCommandBehavior;
using LedMusic2.Enums;
using LedMusic2.Nodes;
using LedMusic2.ViewModels;
using LedMusic2.Views;
using System;

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

        public NodeInterface(string name, ConnectionType ctype, NodeBase parent, bool isInput)
        {
            Name = name;
            ConnectionType = ctype;
            Parent = parent;
            IsInput = isInput;
            _cmdEllipseClicked.ExecuteDelegate = (o) => MainViewModel.Instance.CreateTemporaryConnection(this);
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
            Value = initialValue;
        }

        public override bool SetValue(object input)
        {

            if (input == null)
            {
                return false;
            }
            else if (NodeType.IsAssignableFrom(input.GetType()))
            {
                Value = (T)input;
                base.OnValueChanged(); //TODO: Optimization -> fire event only, if value really changed. (oldVal != newVal)
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
