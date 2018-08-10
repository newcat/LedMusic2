using LedMusic2.Reactive;
using LedMusic2.LedColors;
using System;
using System.Xml.Linq;

namespace LedMusic2.Outputs
{
    public abstract class OutputBase : ReactiveObject, ICombinedReactive, IExportable
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public abstract string DefaultName { get; }
        public ReactivePrimitive<string> Name { get; }
            = new ReactivePrimitive<string>();

        protected OutputBase()
        {
            Name.Set(DefaultName);
        }

        public abstract void CalculationDone(LedColorArray calculationResult);

        public XElement GetXmlElement()
        {

            var el = new XElement("output");
            el.SetAttributeValue("type", DefaultName);
            el.SetAttributeValue("name", Name.Get());
            el.SetAttributeValue("id", Id);
            SaveAdditionalXmlData(el);

            return el;

        }

        public void LoadFromXml(XElement element)
        {

            Name.Set(element.Attribute("name").Value);
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
