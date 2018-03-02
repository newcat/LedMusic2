using System.Collections.Generic;

namespace LedMusic2.NodeTree
{
    class TreeNode<T>
    {

        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();

        public TreeNode(T data)
        {
            Data = data;
        }

    }
}
