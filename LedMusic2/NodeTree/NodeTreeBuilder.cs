using LedMusic2.NodeConnection;
using LedMusic2.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LedMusic2.NodeTree
{
    class NodeTreeBuilder
    {

        private Dictionary<NodeBase, IEnumerable<NodeBase>> adjacency = new Dictionary<NodeBase, IEnumerable<NodeBase>>();
        private List<NodeBase> calculationOrder = new List<NodeBase>();
        private List<NodeBase> outputs = new List<NodeBase>();

        public void Calculate(int index)
        {

            if (index < 0)
                return;

            for (var i = index; i < calculationOrder.Count; i++)
                calculationOrder[i].Calculate();

            foreach (var o in outputs) o.Calculate();

        }

        public void Calculate(NodeBase startingNode)
        {
            Calculate(calculationOrder.IndexOf(startingNode));
        }

        /** Calculates all nodes */
        public void Calculate() { Calculate(0); }

        public void Build(IEnumerable<NodeBase> nodes, IEnumerable<Connection> connections)
        {

            adjacency.Clear();
            calculationOrder.Clear();
            outputs.Clear();

            //Build our adjacency list
            foreach (var n in nodes)
            {
                adjacency.Add(n, connections
                    .Where(c => c.Output != null && c.Input != null && c.Output.Parent == n)
                    .Select(c => c.Input.Parent).AsEnumerable());
            }

            //DFS for initial tree building and cycle detection
            var root = new TreeNode<NodeBase>(null);
            outputs = GetOutputs(nodes);
            root.Children.AddRange(outputs
                .Select(o => adjacency[o])
                .SelectMany(n => n)
                .Select(n => new TreeNode<NodeBase>(n))
                .Distinct());

            FindDescendants(root, new Stack<NodeBase>());

            //BFS with stack to find calculation order
            var queue = new Queue<TreeNode<NodeBase>>();
            var stack = new Stack<NodeBase>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var child in current.Children)
                {
                    stack.Push(child.Data);
                    queue.Enqueue(child);
                }
            }

            //Pop stack to reverse the order
            while (stack.Count > 0)
            {
                var n = stack.Pop();
                if (!calculationOrder.Contains(n))
                    calculationOrder.Add(n);
            }

            calculationOrder.Select(x => x);

        }

        private void FindDescendants(TreeNode<NodeBase> treeNode, Stack<NodeBase> ancestors)
        {
            foreach (var child in treeNode.Children)
            {

                if (ancestors.Contains(child.Data))
                    throw new CyclicGraphException();

                ancestors.Push(child.Data);

                child.Children.AddRange(adjacency[child.Data]
                    .Select(n => new TreeNode<NodeBase>(n)));
                FindDescendants(child, ancestors);

                ancestors.Pop();

            }
        }

        private List<NodeBase> GetOutputs(IEnumerable<NodeBase> nodes)
        {
            return nodes.Where((x) =>
                x.GetType().GetCustomAttribute<NodeAttribute>()?.Category == NodeCategory.OUTPUT).ToList();
        }

    }
}
