﻿using LedMusic2.Attributes;
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
    public abstract class NodeBase : VMBase //TODO: Implement IDisposable
    {

        public static event EventHandler UnselectAllNodes;
        public static void InvokeUnselectAllNodes(object sender)
        {
            UnselectAllNodes?.Invoke(sender, new EventArgs());
        }

        public static event EventHandler OutputChanged;
        public static bool FireOutputChangedEvents = true;
        public void InvokeOutputChanged()
        {
            if (FireOutputChangedEvents)
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

        private Guid _id = Guid.NewGuid();
        public Guid Id { get { return _id; } set { _id = value; } }

        public NodeBase(Point initPosition)
        {
            MainViewModel.Instance.PropertyChanged += MainVM_PropertyChanged;

            PosX = initPosition.X;
            PosY = initPosition.Y;
        }

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
                            loadOption(nodeOptionX);
                        break;

                    case "customdata":
                        LoadAdditionalXmlData(el);
                        break;

                }
            }

        }

        private void loadOption(XElement nodeOptionX)
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
