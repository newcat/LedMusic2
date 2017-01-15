using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2.Models
{
    public class NodeType
    {

        public string Name { get; set; }
        public Type T { get; set; }

        public NodeType(string name, Type t)
        {
            Name = name;
            T = t;
        }

    }
}
