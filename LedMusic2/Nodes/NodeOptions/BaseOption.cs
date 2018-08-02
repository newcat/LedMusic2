using LedMusic2.Reactive;
using System;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public abstract class BaseOption : ReactiveObject, IReactiveListItem, IExportable
    {

        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>("");
        public ReactivePrimitive<NodeOptionType> Type { get; }

        public BaseOption(string name, NodeOptionType type)
        {
            Name.Set(name);
            Type = new ReactivePrimitive<NodeOptionType>(type);
        }

        public virtual XElement GetXmlElement()
        {
            XElement nodeOptionX = new XElement("nodeoption");
            nodeOptionX.SetAttributeValue("type", (int)Type.Get());
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
