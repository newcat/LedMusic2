using LedMusic2.Reactive;
using System;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public abstract class BaseOption : ReactiveObject, IReactiveListItem, IExportable
    {

        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>("");
        public abstract NodeOptionType Type { get; }

        public BaseOption(string name)
        {
            Name.Set(name);
        }

        public virtual XElement GetXmlElement()
        {
            XElement nodeOptionX = new XElement("nodeoption");
            nodeOptionX.SetAttributeValue("type", (int)Type);
            nodeOptionX.SetAttributeValue("name", Name);
            return nodeOptionX;
        }

        public virtual void LoadFromXml(XElement element)
        {
            foreach (XElement el in element.Elements())
            {
                switch (el.Name.LocalName)
                {
                    case "name":
                        Name.Set(el.Value);
                        break;
                }
            }
        }

    }
}
