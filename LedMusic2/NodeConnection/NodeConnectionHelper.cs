using LedMusic2.Nodes;
using LedMusic2.NodeTree;
using LedMusic2.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.NodeConnection
{
    public sealed class NodeConnectionHelper : VMBase
    {

        #region Singleton and Constructor
        private static NodeConnectionHelper _instance = new NodeConnectionHelper();
        public static NodeConnectionHelper Instance { get { return _instance; } }

        private NodeConnectionHelper()
        {

        }
        #endregion

        private NodeInterface _connectionOrigin;
        public NodeInterface ConnectionOrigin
        {
            get { return _connectionOrigin; }
            set
            {
                _connectionOrigin = value;
                NotifyPropertyChanged();
            }
        }

        public bool CanConnect(NodeInterface ni, IEnumerable<NodeBase> nodes, IEnumerable<Connection> connections)
        {

            if (ConnectionOrigin == null)
                return false;

            if (ConnectionOrigin == ni)
                return false;

            if (ConnectionOrigin.Parent == ni.Parent)
                return false;

            //Make sure that one part of the connection is an output and the other one is an input
            if (ConnectionOrigin.IsInput == ni.IsInput)
                return false;

            //find input and output
            NodeInterface input = ni.IsInput ? ni : ConnectionOrigin;
            NodeInterface output = ni.IsInput ? ConnectionOrigin : ni;

            //Check if types match or can be converted
            if (!TypeConverter.CanConvert(output.NodeType, input.NodeType))
                return false;

            //Build a temporary node tree with the connection established and see, if this would
            //lead to a recursion in the tree.
            //Using the ToList()-Method for creating a shallow copy, since we dont want to add
            //the temporary connection to the real node tree.
            var tempConnections = connections.ToList();
            var connectionToTest = new Connection(output, input);
            tempConnections.Add(connectionToTest);
            try
            {
                NodeTreeBuilder ntb = new NodeTreeBuilder();
                ntb.Build(nodes, tempConnections);
            } catch (CyclicGraphException)
            {
                return false;
            } finally
            {
                connectionToTest.Dispose();
            }

            return true;

        }

    }
}
