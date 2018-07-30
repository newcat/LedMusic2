using LedMusic2.BrowserInterop;
using LedMusic2.LedColors;
using System;
using System.Xml.Linq;

namespace LedMusic2.Outputs
{
    public abstract class OutputBase : ReactiveObject, IReactiveListItem, IExportable
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public override string ReactiveName => "Output";

        public abstract string DefaultName { get; }
        public ReactivePrimitive<string> Name = new ReactivePrimitive<string>("OutputName");


        protected OutputBase()
        {
            Name.Set(DefaultName);
        }

        public abstract void CalculationDone(LedColor[] calculationResult);

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
