using LedMusic2.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace LedMusic2.Models
{
    public class Connection : VMBase, IDisposable
    {

        #region ViewModel Properties
        private Point _point0 = new Point(0, 0);
        public Point Point0
        {
            get { return _point0; }
            set
            {
                _point0 = value;
                NotifyPointsChanged();
            }
        }

        public Point Point1
        {
            get { return new Point(Point0.X + (Point3.X - Point0.X) / 2, Point0.Y); }
        }

        public Point Point2
        {
            get { return new Point(Point0.X + (Point3.X - Point0.X) / 2, Point3.Y); }
        }

        private Point _point3 = new Point(0, 0);
        public Point Point3
        {
            get { return _point3; }
            set
            {
                _point3 = value;
                NotifyPointsChanged();
            }
        }

        public int ZIndex { get { return 0; } }
        #endregion

        public NodeInterface Input { get; private set; }
        public NodeInterface Output { get; private set; }

        public static int count = 0;
        private int id = 0;

        private Canvas canvas = null;
        private Ellipse ellInput = null;
        private Ellipse ellOutput = null;

        /// <summary>
        /// Creates a connection between two node interfaces.
        /// </summary>
        /// <param name="input">The output of a node.</param>
        /// <param name="output">The input of another node.</param>
        public Connection(NodeInterface input, NodeInterface output)
        {
            id = count++;
            SetInput(input);
            SetOutput(output);
            transferData();
            CalculatePoints();
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

        private void NodeBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PosX" || e.PropertyName == "PosY" || e.PropertyName == "View")
            {
                CalculatePoints();
            }
        }

        private void NodeInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View")
                CalculatePoints();
        }

        public void CalculatePoints(bool tryAgain = true)
        {

            if (Input.View == null || Output.View == null)
                return; 

            if (ellInput == null) ellInput = Input.View.FindDescendent<Ellipse>().FirstOrDefault();
            if (ellOutput == null) ellOutput = Output.View.FindDescendent<Ellipse>().FirstOrDefault();
            if (ellInput == null || ellOutput == null) return;

            if (canvas == null) canvas = ellInput.FindParent<Canvas>();
            if (canvas == null) return;

            try
            {
                Point0 = ellInput.TransformToVisual(canvas).Transform(new Point(ellInput.ActualWidth / 2, ellInput.ActualHeight / 2));
                Point3 = ellOutput.TransformToVisual(canvas).Transform(new Point(ellOutput.ActualWidth / 2, ellOutput.ActualHeight / 2));
            } catch (InvalidOperationException) {
                ellInput = null;
                ellOutput = null;
                if (tryAgain) CalculatePoints(false);
            }

        }

        public void SetInput(NodeInterface ni)
        {
            if (Input != null)
            {
                Input.ValueChanged -= Input_ValueChanged;
                Input.Parent.PropertyChanged -= NodeBase_PropertyChanged;
                Input.PropertyChanged -= NodeInterface_PropertyChanged;
            }

            ellInput = null;
            Input = ni;
            ni.ValueChanged += Input_ValueChanged;
            Input.Parent.PropertyChanged += NodeBase_PropertyChanged;
            Input.PropertyChanged += NodeInterface_PropertyChanged;
        }

        public void SetOutput(NodeInterface ni)
        {
            if (Output != null)
            {
                Output.Parent.PropertyChanged -= NodeBase_PropertyChanged;
                Output.PropertyChanged -= NodeInterface_PropertyChanged;
                Output.IsConnected = false;
            }

            ellOutput = null;
            Output = ni;
            Output.Parent.PropertyChanged += NodeBase_PropertyChanged;
            Output.PropertyChanged += NodeInterface_PropertyChanged;
            Output.IsConnected = true;
        }

        private void Input_ValueChanged(object sender, EventArgs e)
        {
            transferData();
        }

        private void transferData()
        {
            Output.SetValue(Helpers.TypeConverter.Convert(Input.NodeType, Output.NodeType, Input.GetValue()));
        }

        private void NotifyPointsChanged()
        {
            NotifyPropertyChanged("Point0");
            NotifyPropertyChanged("Point1");
            NotifyPropertyChanged("Point2");
            NotifyPropertyChanged("Point3");
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
                    {
                        Output.IsConnected = false;
                        Output.PropertyChanged -= NodeInterface_PropertyChanged;
                        if (Output.Parent != null)
                            Output.Parent.PropertyChanged -= NodeBase_PropertyChanged;
                    }

                    if (Input != null)
                    {
                        Input.ValueChanged -= Input_ValueChanged;
                        Input.PropertyChanged -= NodeInterface_PropertyChanged;
                        if (Input.Parent != null)
                            Input.Parent.PropertyChanged -= NodeBase_PropertyChanged;
                    }

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
