using LedMusic2.Reactive;
using LedMusic2.Nodes;
using System;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace LedMusic2.NodeConnection
{
    public class Connection : ReactiveObject, IReactiveListItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public ReactivePrimitive<string> InputNodeId { get; }
            = new ReactivePrimitive<string>();
        public ReactivePrimitive<string> InputInterfaceId { get; }
            = new ReactivePrimitive<string>();

        public ReactivePrimitive<string> OutputNodeId { get; }
            = new ReactivePrimitive<string>();
        public ReactivePrimitive<string> OutputInterfaceId { get; }
            = new ReactivePrimitive<string>();

        [ReactiveIgnore]
        public NodeInterface Input { get; private set; }
        [ReactiveIgnore]
        public NodeInterface Output { get; private set; }

        /// <summary>
        /// Creates a connection between two node interfaces.
        /// </summary>
        /// <param name="input">The output of a node.</param>
        /// <param name="output">The input of another node.</param>
        public Connection(NodeInterface input, NodeInterface output)
        {
            SetInput(input);
            SetOutput(output);
            transferData();
        }

        public Connection(JToken j)
        {
            LoadState(j);
        }

        public void Destroy()
        {
            if (Output != null)
                Output.IsConnected.Set(false);

            if (Input != null)
                Input.ValueChanged -= input_ValueChanged;

            Input = null;
            Output = null;
        }

        public void SetInput(NodeInterface ni)
        {
            if (Input != null)
                Input.ValueChanged -= input_ValueChanged;
            Input = ni;
            ni.ValueChanged += input_ValueChanged;
            InputNodeId.Set(ni.Parent.Id.ToString());
            InputInterfaceId.Set(ni.Id.ToString());
        }

        public void SetOutput(NodeInterface ni)
        {
            if (Output != null)
                Output.IsConnected.Set(false);
            Output = ni;
            Output.IsConnected.Set(true);
            OutputNodeId.Set(ni.Parent.Id.ToString());
            OutputInterfaceId.Set(ni.Id.ToString());
        }

        private void input_ValueChanged(object sender, EventArgs e)
        {
            transferData();
        }

        private void transferData()
        {
            Output.SetValue(TypeConverter.Convert(Input.NodeType, Output.NodeType, Input.GetValue()));
        }

    }

}
