﻿using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public abstract class BaseOption : ReactiveObject, IReactiveListItem, IExportable
    {

        public Guid Id { get; } = Guid.NewGuid();

        public ReactivePrimitive<string> Name { get; } = new ReactivePrimitive<string>("");
        public ReactivePrimitive<NodeOptionType> Type { get; } = new ReactivePrimitive<NodeOptionType>(NodeOptionType.BOOL);

        public event EventHandler ValueChanged;

        public BaseOption() { }

        public BaseOption(string name, NodeOptionType type)
        {
            Name.Set(name);
            Type.Set(type);
        }

        protected override void Initialize()
        {
            RegisterCommand("setValue", (p) => {
                SetValue(p);
                RaiseValueChanged();
            });
        }

        public abstract object GetValue();
        protected abstract void SetValue(JToken value);

        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
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
