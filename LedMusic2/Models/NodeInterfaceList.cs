using System;
using System.Collections.Generic;

namespace LedMusic2.Models
{
    public class NodeInterfaceList : List<NodeInterface>
    {

        public event EventHandler<NodeInterfaceValueChangedEventArgs> NodeInterfaceValueChanged;

        public NodeInterface GetNodeInterface(string name)
        {
            foreach (NodeInterface i in this)
            {
                if (i.Name == name)
                    return i;
            }
            return null;
        }

        public new void Add(NodeInterface ni)
        {
            ni.ValueChanged += NodeInterface_ValueChanged;
            base.Add(ni);            
        }

        public new void Clear()
        {
            foreach (NodeInterface ni in this)
                ni.ValueChanged -= NodeInterface_ValueChanged;

            base.Clear();
        }

        public new void Remove(NodeInterface ni)
        {
            ni.ValueChanged -= NodeInterface_ValueChanged;
            base.Remove(ni);
        }

        private void NodeInterface_ValueChanged(object sender, EventArgs e)
        {
            NodeInterface ni = sender as NodeInterface;
            if (ni != null)
                NodeInterfaceValueChanged?.Invoke(this, new NodeInterfaceValueChangedEventArgs(ni.Name));
        }

    }

    public class NodeInterfaceValueChangedEventArgs : EventArgs
    {
        public string InterfaceName { get; private set; }

        public NodeInterfaceValueChangedEventArgs(string interfaceName)
        {
            InterfaceName = interfaceName;
        }
    }

}
