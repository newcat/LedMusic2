using LedMusic2.Attributes;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace LedMusic2.Nodes
{
    public abstract class NodeBase : VMBase //TODO: Implement IDisposable
    {

        public static event EventHandler UnselectAllNodes;
        public static void InvokeUnselectAllNodes(object sender)
        {
            UnselectAllNodes?.Invoke(sender, new EventArgs());
        }

        public static event EventHandler OutputChanged;
        public static bool FireOutputChangedEvents = true;
        protected void InvokeOutputChanged()
        {
            OutputChanged?.Invoke(this, EventArgs.Empty);
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

        private int _minWidth = 0;
        public int MinWidth
        {
            get { return _minWidth; }
            set
            {
                _minWidth = value;
                NotifyPropertyChanged();
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

        protected NodeInterfaceList _inputs = new NodeInterfaceList();
        public NodeInterfaceList Inputs { get { return _inputs; } }

        protected NodeInterfaceList _outputs = new NodeInterfaceList();
        public NodeInterfaceList Outputs { get { return _outputs; } }

        protected ObservableCollection<NodeOptionViewModel> _options = new ObservableCollection<NodeOptionViewModel>();
        public ObservableCollection<NodeOptionViewModel> Options { get { return _options; } }

        public NodeBase(Point initPosition)
        {
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

        public abstract bool Calculate();

    }
}
