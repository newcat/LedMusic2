using LedMusic2.BrowserInterop;
using LedMusic2.LedColors;
using LedMusic2.NodeConnection;
using LedMusic2.Nodes.NodeOptions;
using System;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{
    public abstract class NodeBase : ReactiveObject, IReactiveListItem, IExportable
    {

        public Guid Id { get; set; }
        public override string ReactiveName => "NodeBase";

        public virtual string NodeName
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

        public NodeInterfaceList Inputs { get; } = new NodeInterfaceList();
        public NodeInterfaceList Outputs { get; } = new NodeInterfaceList();
        public ReactiveCollection<BaseOption> Options { get; } = new ReactiveCollection<BaseOption>("Options");

        public abstract bool Calculate();

        protected NodeInterface<T> AddInput<T>(string name)
        {
            var ni = new NodeInterface<T>(name, inferConnectionType<T>(), this, true);
            Inputs.Add(ni);
            return ni;
        }
        protected NodeInterface<T> AddInput<T>(string name, T initialValue)
        {
            var ni = new NodeInterface<T>(name, inferConnectionType<T>(), this, true, initialValue);
            Inputs.Add(ni);
            return ni;
        }

        protected NodeInterface<T> AddOutput<T>(string name)
        {
            var ni = new NodeInterface<T>(name, inferConnectionType<T>(), this, false);
            Outputs.Add(ni);
            return ni;
        }

        private ConnectionType inferConnectionType<T>()
        {

            if (typeof(T) == typeof(double))
                return ConnectionType.NUMBER;
            else if (typeof(T) == typeof(LedColor))
                return ConnectionType.COLOR;
            else if (typeof(T) == typeof(LedColor[]))
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
            foreach (BaseOption opt in Options)
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
            foreach (BaseOption opt in Options)
            {
                if (opt.Name.Get() == name)
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

    }
}
