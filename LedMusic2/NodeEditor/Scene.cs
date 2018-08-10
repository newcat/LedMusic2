using LedMusic2.Reactive;
using LedMusic2.NodeConnection;
using LedMusic2.Nodes;
using LedMusic2.NodeTree;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace LedMusic2.NodeEditor
{
    public class Scene : ReactiveObject, IReactiveListItem, IExportable, IDisposable
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        private NodeTreeBuilder ntb = new NodeTreeBuilder();

        public ReactivePrimitive<string> Name { get; }
            = new ReactivePrimitive<string>("Scene");
        public ReactivePrimitive<TemporaryConnectionState> TemporaryConnectionState { get; }
            = new ReactivePrimitive<TemporaryConnectionState>(NodeEditor.TemporaryConnectionState.NONE);

        public ReactiveCollection<NodeBase> Nodes { get; } = new ReactiveCollection<NodeBase>();
        public ReactiveCollection<Connection> Connections { get; } = new ReactiveCollection<Connection>();
        public ReactiveCollection<NodeType> NodeTypes { get; } = new ReactiveCollection<NodeType>();

        public Scene()
        {
            fillNodeCategories();
            RegisterCommand("addNode", (t) => addNode(t));
            RegisterCommand("deleteNode", (id) => deleteNode(id));
            RegisterCommand("checkTemporaryConnection", (p) => canConnect(p));
            RegisterCommand("addConnection", (p) => createConnection(p));
            RegisterCommand("deleteConnection", (cid) => deleteConnection(cid));
        }

        private void createConnection(JToken payload)
        {
            canConnect(payload);
            var request = (JObject)payload;
            if (TemporaryConnectionState.Get() == NodeEditor.TemporaryConnectionState.ALLOWED)
            {
                var origin = findInterfaceById(Guid.Parse((string)request["originInterfaceId"]));
                var target = findInterfaceById(Guid.Parse((string)request["targetInterfaceId"]));
                var input = origin.IsInput.Get() ? target : origin;
                var output = origin.IsInput.Get() ? origin : target;
                Connections.Add(new Connection(input, output));
                ntb.Build(Nodes, Connections);
            }
        }

        private void deleteConnection(JToken payload)
        {
            var cid = payload.Value<string>();
            var conn = Connections.FindById(cid);
            if (conn != null)
            {
                Connections.Remove(conn);
                conn.Dispose();
            }
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

        private void canConnect(JToken payload)
        {

            if (payload == null)
            {
                TemporaryConnectionState.Set(NodeEditor.TemporaryConnectionState.NONE);
                return;
            }

            var request = (JObject)payload;

            var origin = findInterfaceById(Guid.Parse((string)request["originInterfaceId"]));
            var target = findInterfaceById(Guid.Parse((string)request["targetInterfaceId"]));

            if (origin == null || target == null || origin == target ||
                origin.Parent == target.Parent || origin.IsInput.Get() == target.IsInput.Get())
            {
                TemporaryConnectionState.Set(NodeEditor.TemporaryConnectionState.FORBIDDEN);
                return;
            }

            var input = origin.IsInput.Get() ? target : origin;
            var output = origin.IsInput.Get() ? origin : target;

            if (canConnect(input, output))
                TemporaryConnectionState.Set(NodeEditor.TemporaryConnectionState.ALLOWED);
            else
                TemporaryConnectionState.Set(NodeEditor.TemporaryConnectionState.FORBIDDEN);

        }

        private bool canConnect(NodeInterface input, NodeInterface output)
        {

            if (!TypeConverter.CanConvert(input.NodeType, output.NodeType))
                return false;

            //Build a temporary node tree with the connection established and see, if this would
            //lead to a recursion in the tree.
            //Using the ToList()-Method for creating a shallow copy, since we dont want to add
            //the temporary connection to the real node tree.
            var tempConnections = Connections.ToList();
            var connectionToTest = new Connection(input, output);
            tempConnections.Add(connectionToTest);
            try
            {
                NodeTreeBuilder ntb = new NodeTreeBuilder();
                ntb.Build(Nodes, tempConnections);
            }
            catch (CyclicGraphException)
            {
                return false;
            }
            finally
            {
                connectionToTest.Dispose();
            }

            return true;

        }

        #region Nodes
        private void deleteNode(JToken payload)
        {
            
            var node = Nodes.FindById((payload as JToken).Value<string>());
            if (node == null) return;

            //Delete all the connections from and to the node.
            var toDelete = Connections.Where((x) => x.Input.Parent == node || x.Output.Parent == node).ToArray();

            foreach (Connection c in toDelete)
            {
                Connections.Remove(c);
                c.Dispose();
            }

            Nodes.Remove(node);

        }

        private NodeBase addNode(JToken payload)
        {

            var t = NodeTypes.FindById((payload as JToken).Value<string>());
            if (t == null) return null;

            var constructor = t.T.GetConstructor(new Type[] {});
            var node = (NodeBase)constructor.Invoke(new object[] {});
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
                if (n.Name.Get() == type)
                {
                    nodeInstance = addNode(n.Id);
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