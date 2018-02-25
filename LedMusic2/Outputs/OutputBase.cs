using LedMusic2.Color;
using LedMusic2.ViewModels;
using System;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.Outputs
{
    public abstract class OutputBase : VMBase, IExportable
    {

        public abstract string DefaultName { get; }

        public abstract FrameworkElement SettingsView { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        protected OutputBase()
        {
            Name = DefaultName;
        }

        public abstract void CalculationDone(LedColor[] calculationResult);

        public XElement GetXmlElement()
        {

            var el = new XElement("output");
            el.SetAttributeValue("type", DefaultName);
            el.SetAttributeValue("name", Name);
            el.SetAttributeValue("id", Id);
            SaveAdditionalXmlData(el);

            return el;

        }

        public void LoadFromXml(XElement element)
        {

            Name = element.Attribute("name").Value;
            Id = Guid.Parse(element.Attribute("id").Value);
            LoadAdditionalXmlData(element);

        }

        protected virtual void SaveAdditionalXmlData(XElement x)
        {
            return;
        }

        protected virtual void LoadAdditionalXmlData(XElement x)
        {
            return;
        }

    }
}
