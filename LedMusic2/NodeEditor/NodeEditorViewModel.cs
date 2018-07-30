using LedMusic2.BrowserInterop;
using LedMusic2.NodeConnection;
using LedMusic2.Nodes;
using LedMusic2.NodeTree;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace LedMusic2.NodeEditor
{
    public class NodeEditorViewModel : ReactiveObject, IReactiveListItem, IExportable, IDisposable
    {

        public Guid Id { get; set; }
        public override string ReactiveName => "NodeEditorViewModel";

        private NodeTreeBuilder ntb = new NodeTreeBuilder();
        public ReactiveCollection<NodeBase> Nodes { get; } = new ReactiveCollection<NodeBase>("Nodes");
        public ReactiveCollection<Connection> Connections { get; } = new ReactiveCollection<Connection>("Connections");
        public ReactiveCollection<NodeType> NodeTypes { get; } = new ReactiveCollection<NodeType>("NodeTypes");


        public NodeEditorViewModel()
        {
            fillNodeCategories();
        }

        public void CreateConnection()
        {
            throw new NotImplementedException();
        }

        private void fillNodeCategories()
        {

            //Get all node classes
            var nodeClasses = Assembly.GetCallingAssembly().GetTypes().Where((t) => string.Equals(t.Namespace, "LedMusic2.Nodes.NodeModels") && !t.IsAbstract);

            var nodes = nodeClasses.Where((t) => t.GetCustomAttribute(typeof(NodeAttribute)) != null);
            foreach (var node in nodes)
            {
                var attribute = (NodeAttribute)node.GetCustomAttribute(typeof(NodeAttribute));
                NodeTypes.Add(new NodeType(attribute.Name, attribute.Category, node));
            }

        }

        #region Nodes
        public void DeleteNode(NodeBase node)
        {

            if (node == null)
                return;

            //Delete all the connections from and to the node.
            var toDelete = Connections.Where((x) => x.Input.Parent == node || x.Output.Parent == node).ToArray();

            foreach (Connection c in toDelete)
            {
                Connections.Remove(c);
                c.Dispose();
            }

            Nodes.Remove(node);

        }

        public NodeBase AddNode(NodeType t)
        {

            if (t == null)
                return null;

            var constructor = t.T.GetConstructor(new Type[] { typeof(Point), typeof(NodeEditorViewModel) });
            var node = (NodeBase)constructor.Invoke(new object[] { new Point(0, 0), this });
            Nodes.Add(node);
            return node;

        }

        private void calculateNodeTree()
        {
            ntb.Build(Nodes, Connections);
        }

        public void CalculateNodes(NodeBase startingNode)
        {
            ntb.Calculate(startingNode);
        }

        public void CalculateNodes(int startingIndex)
        {
            ntb.Calculate(startingIndex);
        }

        public void CalculateAllNodes()
        {
            CalculateNodes(0);
        }
        #endregion

        #region Save and Load
        public XElement GetXmlElement()
        {

            var rootX = new XElement("scene");

            XElement nodesX = new XElement("nodes");
            foreach (NodeBase n in Nodes)
            {
                nodesX.Add(n.GetXmlElement());
            }
            rootX.Add(nodesX);

            XElement connectionsX = new XElement("connections");
            foreach (Connection c in Connections)
            {
                connectionsX.Add(c.GetXmlElement());
            }
            rootX.Add(connectionsX);

            return rootX;

        }

        public void LoadFromXml(XElement element)
        {

            foreach (XElement n in element.Elements())
            {
                switch (n.Name.LocalName)
                {

                    case "nodes":
                        foreach (XElement nodeX in n.Elements())
                            loadNode(nodeX);
                        break;

                    case "connections":
                        foreach (XElement connectionX in n.Elements())
                            loadConnection(connectionX);
                        break;

                }
            }

        }

        private void loadNode(XElement nodeX)
        {

            string type = nodeX.Attribute("type").Value;
            NodeBase nodeInstance = null;
            foreach (NodeType n in NodeTypes)
            {
                if (n.Name == type)
                {
                    nodeInstance = AddNode(n);
                    break;
                }
            }
            if (nodeInstance != null) nodeInstance.LoadFromXml(nodeX);

        }

        private void loadConnection(XElement connectionX)
        {

            Guid inputId = Guid.Empty, outputId = Guid.Empty;

            foreach (XElement el in connectionX.Elements())
            {
                if (el.Name.LocalName == "input")
                    inputId = Guid.Parse(el.Attribute("interfaceid").Value);
                else if (el.Name.LocalName == "output")
                    outputId = Guid.Parse(el.Attribute("interfaceid").Value);
            }

            if (inputId == Guid.Empty || outputId == Guid.Empty)
                return;

            NodeInterface input = findInterfaceById(inputId);
            NodeInterface output = findInterfaceById(outputId);

            if (input == null || output == null)
                return;

            Connection c = new Connection(input, output);
            Connections.Add(c);

        }

        private NodeInterface findInterfaceById(Guid id)
        {
            foreach (NodeBase n in Nodes)
            {
                foreach (NodeInterface ni in n.Inputs)
                {
                    if (ni.Id == id)
                        return ni;
                }
                foreach (NodeInterface ni in n.Outputs)
                {
                    if (ni.Id == id)
                        return ni;
                }
            }
            return null;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (Connection c in Connections)
                        c.Dispose();
                    Connections.Clear();
                    Nodes.Clear();
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