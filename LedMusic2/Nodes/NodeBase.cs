using LedMusic2.Attributes;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace LedMusic2.Nodes
{
    public abstract class NodeBase : INotifyPropertyChanged //TODO: Implement IDisposable
    {

        public static event EventHandler UnselectAllNodes;
        public static void InvokeUnselectAllNodes(object sender)
        {
            UnselectAllNodes?.Invoke(sender, new EventArgs());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region ViewModel Properties
        private double _posX;
        public double PosX
        {
            get { return _posX + MainViewModel.Instance.TranslateX; }
            set
            {
                _posX = value;
                NotifyPropertyChanged();
            }
        }

        private double _posY;
        public double PosY
        {
            get { return _posY + MainViewModel.Instance.TranslateY; }
            set
            {
                _posY = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ZIndex");
            }
        }

        public int ZIndex
        {
            get { return IsSelected ? 2 : 1; }
        }

        public virtual string Name
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
        #endregion

        public static bool FireInputChangedEvents { get; set; }
        public static event EventHandler InputChanged;

        protected NodeInterfaceList _inputs = new NodeInterfaceList();
        public NodeInterfaceList Inputs { get { return _inputs; } }

        protected NodeInterfaceList _outputs = new NodeInterfaceList();
        public NodeInterfaceList Outputs { get { return _outputs; } }

        protected ObservableCollection<NodeOptionViewModel> _options = new ObservableCollection<NodeOptionViewModel>();
        public ObservableCollection<NodeOptionViewModel> Options { get { return _options; } }

        public NodeBase(Point initPosition)
        {
            _inputs.NodeInterfaceValueChanged += _inputs_NodeInterfaceValueChanged;
            MainViewModel.Instance.PropertyChanged += MainVM_PropertyChanged;

            PosX = initPosition.X;
            PosY = initPosition.Y;
        }

        private void MainVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TranslateX" || e.PropertyName == "TranslateY")
            {
                NotifyPropertyChanged("PosX");
                NotifyPropertyChanged("PosY");
            }                
        }

        private void _inputs_NodeInterfaceValueChanged(object sender, NodeInterfaceValueChangedEventArgs e)
        {
            if (e.InterfaceName != null && e.InterfaceName != "" &&
                InputValueChanged(e.InterfaceName) && FireInputChangedEvents)
                InputChanged?.Invoke(this, new EventArgs());                
        }

        /// <summary>
        /// This method is purely for optimization. It basically says: "This input value changed, should I recalculate
        /// the output values?". For example, if the alpha value of a node is 0 and the color value changed, it would
        /// be useless to calculate, since the output won't change.
        /// Note: Even if this method returns true, it isn't guranteed to be recalculated, but it will always be called
        /// if a input value changes. If it returns false, it will never calculate.
        /// </summary>
        /// <param name="NodeInterfaceName">The name of the interface that changed its value.</param>
        /// <returns><c>true</c> if the node should recalculate its outputs, <c>false</c> if not.</returns>
        protected abstract bool InputValueChanged(string NodeInterfaceName);

        public abstract bool Calculate();

    }
}
