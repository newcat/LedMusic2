using LedMusic2.BrowserInterop;
using LedMusic2.Nodes;
using LedMusic2.ViewModels;
using System;
using System.Xml.Linq;

namespace LedMusic2.NodeConnection
{
    public class Connection : ReactiveObject, IReactiveListItem, IExportable, IDisposable
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public override string ReactiveName => "Connection";

        public NodeInterface Input { get; private set; }
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

        public XElement GetXmlElement()
        {
            XElement connectionX = new XElement("connection");

            XElement inputX = new XElement("input");
            inputX.SetAttributeValue("interfaceid", Input.Id);

            XElement outputX = new XElement("output");
            outputX.SetAttributeValue("interfaceid", Output.Id);

            connectionX.Add(inputX, outputX);
            return connectionX;
        }

        public void LoadFromXml(XElement element)
        {
            throw new NotImplementedException("Connections cannot be loaded directly from xml.");
        }

        public void SetInput(NodeInterface ni)
        {
            if (Input != null)
                Input.ValueChanged -= input_ValueChanged;
            Input = ni;
            ni.ValueChanged += input_ValueChanged;
        }

        public void SetOutput(NodeInterface ni)
        {
            if (Output != null)
                Output.IsConnected.Set(false);
            Output = ni;
            Output.IsConnected.Set(true);
        }

        private void input_ValueChanged(object sender, EventArgs e)
        {
            transferData();
        }

        private void transferData()
        {
            Output.SetValue(TypeConverter.Convert(Input.NodeType, Output.NodeType, Input.GetValue()));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                    if (Output != null)
                        Output.IsConnected.Set(false);

                    if (Input != null)
                        Input.ValueChanged -= input_ValueChanged;

                    Input = null;
                    Output = null;

                }

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
