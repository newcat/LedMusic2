using LedMusic2.LedColors;
using LedMusic2.NodeConnection;
using LedMusic2.NodeEditor;
using LedMusic2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{
    public abstract class NodeBase : VMBase, IExportable, IDisposable
    {

        public static event EventHandler UnselectAllNodes;
        public static void InvokeUnselectAllNodes(object sender)
        {
            UnselectAllNodes?.Invoke(sender, new EventArgs());
        }

        #region ViewModel Properties
        private double _posX;
        public double PosX
        {
            get { return _posX + NodeEditorVM.TranslateX; }
            set
            {
                _posX = value;
                NotifyPropertyChanged();
            }
        }

        private double _posY;
        public double PosY
        {
            get { return _posY + NodeEditorVM.TranslateY; }
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

        protected ObservableCollection<NodeOption> _options = new ObservableCollection<NodeOption>();
        public ObservableCollection<NodeOption> Options { get { return _options; } }

        private Guid _id = Guid.NewGuid();
        public Guid Id { get { return _id; } set { _id = value; } }

        public NodeEditorViewModel NodeEditorVM { get; private set; }

        public NodeBase(Point initPosition, NodeEditorViewModel parentVM)
        {

            NodeEditorVM = parentVM;
            NodeEditorVM.PropertyChanged += NodeEditorVM_PropertyChanged;

            PosX = initPosition.X;
            PosY = initPosition.Y;

        }

        private void NodeEditorVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TranslateX" || e.PropertyName == "TranslateY")
            {
                NotifyPropertyChanged("PosX");
                NotifyPropertyChanged("PosY");
            }                
        }

        public abstract bool Calculate();

        protected NodeInterface<T> AddInput<T>(string name)
        {
            var ni = new NodeInterface<T>(name, InferConnectionType<T>(), this, true, NodeEditorVM);
            Inputs.Add(ni);
            return ni;
        }
        protected NodeInterface<T> AddInput<T>(string name, T initialValue)
        {
            var ni = new NodeInterface<T>(name, InferConnectionType<T>(), this, true, NodeEditorVM, initialValue);
            Inputs.Add(ni);
            return ni;
        }

        protected NodeInterface<T> AddOutput<T>(string name)
        {
            var ni = new NodeInterface<T>(name, InferConnectionType<T>(), this, false, NodeEditorVM);
            Outputs.Add(ni);
            return ni;
        }

        private ConnectionType InferConnectionType<T>()
        {

            if (typeof(T) == typeof(double))
                return ConnectionType.NUMBER;
            else if (typeof(T) == typeof(LedColors.LedColor))
                return ConnectionType.COLOR;
            else if (typeof(T) == typeof(LedColors.LedColor[]))
                return ConnectionType.COLOR_ARRAY;
            else if (typeof(T) == typeof(bool))
                return ConnectionType.BOOL;
            else
                throw new ArgumentException("Invalid type: " + typeof(T).ToString());

        }

        #region Saving and Loading
        public XElement GetXmlElement()
        {

            XElement nodeX = new XElement("node");
            nodeX.SetAttributeValue("type", ((NodeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(NodeAttribute))).Name);
            nodeX.Add(new XElement("id", Id));
            nodeX.Add(new XElement("posx", PosX.ToString(CultureInfo.InvariantCulture)));
            nodeX.Add(new XElement("posy", PosY.ToString(CultureInfo.InvariantCulture)));

            XElement inputsX = new XElement("inputs");
            foreach (NodeInterface ni in Inputs)
            {
                inputsX.Add(ni.GetXmlElement());
            }
            nodeX.Add(inputsX);

            XElement outputsX = new XElement("outputs");
            foreach (NodeInterface ni in Outputs)
            {
                outputsX.Add(ni.GetXmlElement());
            }
            nodeX.Add(outputsX);

            XElement optionsX = new XElement("options");
            foreach (NodeOption opt in Options)
            {
                optionsX.Add(opt.GetXmlElement());
            }
            nodeX.Add(optionsX);

            XElement customX = new XElement("customdata");
            SaveAdditionalXmlData(customX);
            if (customX.HasElements)
                nodeX.Add(customX);

            return nodeX;
            
        }

        public void LoadFromXml(XElement node)
        {

            foreach (XElement el in node.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "id":
                        Id = Guid.Parse(el.Value);
                        break;

                    case "posx":
                        PosX = double.Parse(el.Value, CultureInfo.InvariantCulture);
                        break;

                    case "posy":
                        PosY = double.Parse(el.Value, CultureInfo.InvariantCulture);
                        break;

                    case "inputs":
                        foreach (XElement niX in el.Elements())
                        {
                            NodeInterface ni = Inputs.GetNodeInterface(niX.Attribute("name").Value);
                            if (ni != null)
                                ni.LoadFromXml(niX);
                        }
                        break;

                    case "outputs":
                        foreach (XElement niX in el.Elements())
                        {
                            NodeInterface ni = Outputs.GetNodeInterface(niX.Attribute("name").Value);
                            if (ni != null)
                                ni.LoadFromXml(niX);
                        }
                        break;

                    case "options":
                        foreach (XElement nodeOptionX in el.Elements())
                            LoadOption(nodeOptionX);
                        break;

                    case "customdata":
                        LoadAdditionalXmlData(el);
                        break;

                }
            }

        }

        private void LoadOption(XElement nodeOptionX)
        {
            string name = nodeOptionX.Attribute("name").Value;
            foreach (NodeOption opt in Options)
            {
                if (opt.Name == name)
                    opt.LoadFromXml(nodeOptionX);
            }
        }

        protected virtual void SaveAdditionalXmlData(XElement x)
        {
            return;
        }

        protected virtual void LoadAdditionalXmlData(XElement x)
        {
            return;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    NodeEditorVM.PropertyChanged -= NodeEditorVM_PropertyChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
