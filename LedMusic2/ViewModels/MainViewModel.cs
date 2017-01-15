using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Helpers;
using LedMusic2.Models;
using LedMusic2.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LedMusic2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region Singleton and Constructor
        private static MainViewModel _instance = new MainViewModel();
        public static MainViewModel Instance
        {
            get { return _instance; }
        }
        private MainViewModel()
        {
            NodeBase.UnselectAllNodes += NodeBase_UnselectAllNodes;
        }
        #endregion

        #region Properties
        private double _scale = 1.0;
        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value > 0.3)
                _scale = value;
                NotifyPropertyChanged();
            }
        }

        private double _scaleCenterX = 0.0;
        public double ScaleCenterX
        {
            get { return _scaleCenterX; }
            set
            {
                _scaleCenterX = value;
                NotifyPropertyChanged();
            }
        }

        private double _scaleCenterY = 0.0;
        public double ScaleCenterY
        {
            get { return _scaleCenterY; }
            set
            {
                _scaleCenterY = value;
                NotifyPropertyChanged();
            }
        }

        private double _translateX = 0.0;
        public double TranslateX
        {
            get { return _translateX; }
            set
            {
                _translateX = value;
                NotifyPropertyChanged();
            }
        }

        private double _translateY = 0.0;
        public double TranslateY
        {
            get { return _translateY; }
            set
            {
                _translateY = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<NodeBase> _nodes = new ObservableCollection<NodeBase>();
        public ObservableCollection<NodeBase> Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Connection> _connections = new ObservableCollection<Connection>();
        public ObservableCollection<Connection> Connections
        {
            get { return _connections; }
            set
            {
                _connections = value;
                NotifyPropertyChanged();
            }
        }

        private TemporaryConnectionViewModel _temporaryConnection;
        public TemporaryConnectionViewModel TemporaryConnection
        {
            get { return _temporaryConnection; }
            set
            {
                _temporaryConnection = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TemporaryConnectionOC");
            }
        }

        public ObservableCollection<TemporaryConnectionViewModel> TemporaryConnectionOC
        {
            get { return new ObservableCollection<TemporaryConnectionViewModel>() { TemporaryConnection }; }
        }

        private double _mousePosX = 0;
        public double MousePosX
        {
            get { return _mousePosX; }
            set
            {
                _mousePosX = value;
                NotifyPropertyChanged();
            }
        }

        private double _mousePosY = 0;
        public double MousePosY
        {
            get { return _mousePosY; }
            set
            {
                _mousePosY = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isAddNodePanelOpen = false;
        public bool IsAddNodePanelOpen
        {
            get { return _isAddNodePanelOpen; }
            set
            {
                _isAddNodePanelOpen = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<NodeCategoryModel> _nodeCategories = new ObservableCollection<NodeCategoryModel>();
        public ObservableCollection<NodeCategoryModel> NodeCategories
        {
            get { return _nodeCategories; }
            set
            {
                _nodeCategories = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Public Methods
        public void Initialize()
        {
            fillNodeCategories();
        }

        public void CreateTemporaryConnection(NodeInterface sender)
        {

            if (sender.IsInput && Connections.FirstOrDefault((x) => x.Output == sender) != null)
            {
                //Since the sender interface is an input and there already exists a connection, remove the old
                //connection and make it temporary so the user can either remove it or rewire it.
                var oldConnection = Connections.FirstOrDefault((x) => x.Output == sender);
                var input = oldConnection.Input;
                Connections.Remove(oldConnection);
                TemporaryConnection = new TemporaryConnectionViewModel(input);
                NodeConnectionHelper.Instance.ConnectionOrigin = input;
            } else
            {
                TemporaryConnection = new TemporaryConnectionViewModel(sender);
                NodeConnectionHelper.Instance.ConnectionOrigin = sender;
            }                      

        }        

        public void MouseOverInterface(NodeInterface sender)
        {
            if (TemporaryConnection == null)
                return;

            TemporaryConnection.SetTargetInterface(sender);
            TemporaryConnection.IsConnectionAllowed = NodeConnectionHelper.Instance.CanConnect(sender);
            
        }

        public void MouseLeftInterface()
        {
            if (TemporaryConnection == null)
                return;

            TemporaryConnection.SetTargetInterface(null);
            TemporaryConnection.IsConnectionAllowed = false;
        }

        public void FinalizeTemporaryConnection()
        {

            if (TemporaryConnection.TargetInterface != null && TemporaryConnection.IsConnectionAllowed)
            {

                NodeInterface input, output;
                if (TemporaryConnection.OriginInterface.IsInput)
                {
                    //Input & Output switched in connection, because for the connection
                    //the output of a node is actually the input and vice versa
                    output = TemporaryConnection.OriginInterface;
                    input = TemporaryConnection.TargetInterface;
                } else
                {
                    input = TemporaryConnection.OriginInterface;
                    output = TemporaryConnection.TargetInterface;
                }

                //Check if the output of the connection aka the input of a node already
                //has a connection to it and if so, remove it, because only one connection
                //per input is allowed.
                var oldC = Connections.FirstOrDefault((x) => x.Output == output);
                if (oldC != null)
                    Connections.Remove(oldC);

                var c = new Connection(input, output);
                Connections.Add(c);

            }

            TemporaryConnection.Dispose();
            TemporaryConnection = null;
            NodeConnectionHelper.Instance.ConnectionOrigin = null;

        }

        public void RecalculateAllConnectionPoints()
        {
            foreach (var c in Connections)
                c.CalculatePoints();
        }
        #endregion

        #region Private Methods
        private void NodeBase_UnselectAllNodes(object sender, EventArgs e)
        {
            foreach (NodeBase n in Nodes)
                n.IsSelected = false;
        }

        private void fillNodeCategories()
        {

            //Get all node classes
            var nodeClasses = Assembly.GetCallingAssembly().GetTypes().Where((t) => string.Equals(t.Namespace, "LedMusic2.Nodes") && !t.IsAbstract);
            foreach (NodeCategory category in Enum.GetValues(typeof(NodeCategory)))
            {
                var c = new NodeCategoryModel(Enum.GetName(typeof(NodeCategory), category));
                var n = nodeClasses.Where((t) => {
                        var x = t.GetCustomAttribute(typeof(NodeAttribute));
                        if (x == null)
                            return false;
                        return ((NodeAttribute)x).Category == category;
                    });

                foreach (var x in n)
                    c.NodeTypes.Add(new NodeType(((NodeAttribute)x.GetCustomAttribute(typeof(NodeAttribute))).Name, x));

                NodeCategories.Add(c);
            }

        }
        #endregion

    }
}
