using LedMusic2.Reactive;

namespace LedMusic2.Nodes
{
    public class NodeInterfaceList : ReactiveCollection<NodeInterface>
    {

        public NodeInterface GetNodeInterface(string name)
        {
            foreach (NodeInterface i in this)
            {
                if (i.Name.Get() == name)
                    return i;
            }
            return null;
        }

        public NodeInterface<T> GetNodeInterface<T>(string name) => (NodeInterface<T>)GetNodeInterface(name);

        public new void Add(NodeInterface ni)
        {
            base.Add(ni);            
        }

        public new void Clear()
        {
            base.Clear();
        }

        public new void Remove(NodeInterface ni)
        {
            base.Remove(ni);
        }

    }

}
