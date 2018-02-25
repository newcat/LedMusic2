using LedMusic2.NodeConnection;
using LedMusic2.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LedMusic2.NodeTree
{
    class NodeTreeBuilder : IDisposable
    {

        private List<Layer[]> multilayers = new List<Layer[]>();
        private Layer[] layers;
        private List<Dependency> dependencies = new List<Dependency>();
        private int depth;

        /// <summary>
        /// Returns a list, which is ordered by which node should be calculated first, so the first node in the
        /// list should be calculated first and the last node in the list should be calculated last.
        /// </summary>
        /// <param name="rootElement">The element which should be calculated last, e.g. the output node.</param>
        /// <param name="nodes">The list of nodes in the node system.</param>
        /// <param name="connections">The list of connections between the nodes.</param>
        /// <returns></returns>
        public NodeBase[] GetCalculationOrder(NodeBase[] rootElements, NodeBase[] nodes, Connection[] connections)
        {

            //clear old data
            multilayers.Clear();
            layers = null;
            dependencies.Clear();
            depth = 0;

            //Build dependency database
            foreach (Connection c in connections)
            {
                dependencies.Add(new Dependency(c.Output.Parent, c.Input.Parent));
            }

            //Build the node tree
            foreach (NodeBase rootElement in rootElements)
            {

                depth = 0;
                TreeNode tree;
                try
                {
                    tree = getTree(rootElement, depth);
                }
                catch (RecursionException)
                {
                    throw;
                }

                var localLayers = new Layer[depth];
                createLayers(localLayers, tree, 0);
                multilayers.Add(localLayers);

            }

            combineLayers();            
            simplifyLayers();

            //Create the order
            return createOrder().ToArray();

        }

        public NodeBase[] GetRootElements(NodeBase[] nodes)
        {
            return nodes.Where((x) =>
                x.GetType().GetCustomAttribute<NodeAttribute>()?.Category == NodeCategory.OUTPUT).ToArray();
        }

        #region Private Methods
        private TreeNode getTree(NodeBase rootElement, int d)
        {

            d++;
            if (d > depth)
                depth = d;

            if (depth > 100)
                throw new RecursionException();

            var root = new TreeNode(rootElement);

            foreach (var dep in dependencies.Where((x) => x.Element == rootElement))
            {
                root.Children.Add(getTree(dep.IsDependantOf, d));
            }

            return root;

        }

        private void createLayers(Layer[] arr, TreeNode t, int d)
        {

            if (arr[d] == null)
                arr[d] = new Layer();

            //Dont even bother to add nodes more than one time,
            //they would be removed in the simplify step anyways.
            if (!arr[d].Nodes.Contains(t.Node))
                arr[d].Nodes.Add(t.Node);

            foreach (TreeNode children in t.Children)
            {
                createLayers(arr, children, d + 1);
            }

        }

        private void combineLayers()
        {

            var layersList = new List<Layer>();
            int maxDepth = 1;            
            for (int i = 0; i < maxDepth; i++)
            {

                var combinedLayer = new Layer();

                foreach (Layer[] singleRootLayers in multilayers)
                {

                    if (i == 0 && singleRootLayers.Length > maxDepth)
                        maxDepth = singleRootLayers.Length;

                    if (i < singleRootLayers.Length)
                    {
                        foreach (NodeBase node in singleRootLayers[i].Nodes)
                        {
                            if (!combinedLayer.Nodes.Contains(node))
                                combinedLayer.Nodes.Add(node);
                        }
                    }

                }

                layersList.Add(combinedLayer);
            }

            layers = layersList.ToArray();

        }

        private void simplifyLayers()
        {

            //If a node exists on multiple layers, only keep the one on the highest layer
            //(which will be calculated first, so no need to calculate it again later).
            for (int i = depth - 1; i > 0; i--)
            {
                foreach (NodeBase b in layers[i].Nodes)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        layers[j].Nodes.Remove(b);
                    }
                }
            }

        }

        private List<NodeBase> createOrder()
        {

            var order = new List<NodeBase>();

            for (int i = depth - 1; i >= 0; i--)
            {
                order.AddRange(layers[i].Nodes);
            }

            return order;

        }
        #endregion

        #region Classes
        private class Layer
        {
            public List<NodeBase> Nodes { get; set; }
            
            public Layer()
            {
                Nodes = new List<NodeBase>();
            }
        }

        private class TreeNode
        {
            public NodeBase Node { get; set; }
            public List<TreeNode> Children { get; set; }

            public TreeNode(NodeBase node)
            {
                Node = node;
                Children = new List<TreeNode>();
            }

        }

        private class Dependency
        {
            public NodeBase Element { get; set; }
            public NodeBase IsDependantOf { get; set; }

            public Dependency(NodeBase element, NodeBase isDependantOf)
            {
                Element = element;
                IsDependantOf = isDependantOf;
            }
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
                    multilayers.Clear();
                    dependencies.Clear();
                }

                layers = null;

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
