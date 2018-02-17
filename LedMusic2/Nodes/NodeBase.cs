using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Interfaces;
using LedMusic2.Models;
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

        protected ObservableCollection<NodeOptionViewModel> _options = new ObservableCollection<NodeOptionViewModel>();
        public ObservableCollection<NodeOptionViewModel> Options { get { return _options; } }

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

        protected NodeInterface AddInput(string name, ConnectionType type)
        {
            var ni = CreateInterface(name, type, true);
            Inputs.Add(ni);
            return ni;
        }

        protected NodeInterface AddOutput(string name, ConnectionType type)
        {
            var ni = CreateInterface(name, type, false);
            Outputs.Add(ni);
            return ni;
        }

        private NodeInterface CreateInterface(string name, ConnectionType type, bool isInput)
        {

            Type t = null;
            switch (type)
            {
                case ConnectionType.BOOL:
                    t = typeof(bool);
                    break;
                case ConnectionType.COLOR:
                    t = typeof(LedColor);
                    break;
                case ConnectionType.COLOR_ARRAY:
                    t = typeof(LedColor[]);
                    break;
                case ConnectionType.NUMBER:
                    t = typeof(double);
                    break;
            }

            var niType = typeof(NodeInterface<>).MakeGenericType(t);
            return (NodeInterface)Activator.CreateInstance(niType, new object[] { name, type, this, isInput, NodeEditorVM });

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
            foreach (NodeOptionViewModel opt in Options)
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
            foreach (NodeOptionViewModel opt in Options)
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
