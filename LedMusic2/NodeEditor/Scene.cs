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
    public class Scene : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        private NodeTreeBuilder ntb = new NodeTreeBuilder();

        public ReactivePrimitive<string> Name { get; }
            = new ReactivePrimitive<string>("Scene");
        public ReactivePrimitive<TemporaryConnectionState> TemporaryConnectionState { get; }
            = new ReactivePrimitive<TemporaryConnectionState>(NodeEditor.TemporaryConnectionState.NONE);

        public ReactiveCollection<NodeBase> Nodes { get; } = new ReactiveCollection<NodeBase>();
        public ReactiveCollection<Connection> Connections { get; } = new ReactiveCollection<Connection>();

        [IgnoreOnLoad]
        public ReactiveCollection<NodeType> NodeTypes { get; } = new ReactiveCollection<NodeType>();

        public Scene()
        {
            RegisterCommand("addNode", (t) => addNode(t));
            RegisterCommand("deleteNode", (id) => deleteNode(id));
            RegisterCommand("checkTemporaryConnection", (p) => canConnect(p));
            RegisterCommand("addConnection", (p) => createConnection(p));
            RegisterCommand("deleteConnection", (cid) => deleteConnection(cid));
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();            

            // Fill node types
            NodeTypes.Clear();
            var nodeClasses = Assembly.GetCallingAssembly().GetTypes().Where((t) => string.Equals(t.Namespace, "LedMusic2.Nodes.NodeModels") && !t.IsAbstract);
            NodeTypes.AddRange(nodeClasses
                .Where((t) => t.GetCustomAttribute(typeof(NodeAttribute)) != null)
                .Select((node) => {
                    var attribute = (NodeAttribute)node.GetCustomAttribute(typeof(NodeAttribute));
                    return new NodeType(attribute.Name, attribute.Category, node);
                }
            ));

            // Load Connections
            foreach (var connection in Connections)
            {
                var input = findInterfaceById(Guid.Parse(connection.InputInterfaceId.Get()));
                var output = findInterfaceById(Guid.Parse(connection.OutputInterfaceId.Get()));

                if (input == null || output == null)
                    throw new Exception("Could not find node interface to load connection.");

                connection.SetInput(input);
                connection.SetOutput(output);
            }

        }

        public void Connect(NodeInterface input, NodeInterface output)
        {
            if (canConnect(input, output))
            {
                Connections.Add(new Connection(input, output));
                ntb.Build(Nodes, Connections);
            }
        }

        private void createConnection(JToken payload)
        {
            var request = (JObject)payload;
            var origin = findInterfaceById(Guid.Parse((string)request["originInterfaceId"]));
            var target = findInterfaceById(Guid.Parse((string)request["targetInterfaceId"]));
            var input = origin.IsInput.Get() ? target : origin;
            var output = origin.IsInput.Get() ? origin : target;
            Connect(input, output);
        }

        private void deleteConnection(JToken payload)
        {
            var cid = payload.Value<string>();
            var conn = Connections.FindById(cid);
            if (conn != null)
            {
                Connections.Remove(conn);
                conn.Destroy();
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
                connectionToTest.Destroy();
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
                c.Destroy();
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

        public void CalculateAllNodes()
        {
            ntb.Calculate(0);
        }
        #endregion

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

    }
}