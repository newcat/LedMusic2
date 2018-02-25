using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2.Nodes
{
    public class NodeCategoryModel
    {

        public string Name { get; set; }
        public ObservableCollection<NodeType> NodeTypes { get; set; }

        public NodeCategoryModel(string name)
        {
            NodeTypes = new ObservableCollection<NodeType>();
            Name = name;
        }

    }
}
