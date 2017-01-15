using LedMusic2.Helpers;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LedMusic2.Models
{
    public class Connection : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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
            CalculatePoints();
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

        public void CalculatePoints()
        {

            if (Input.View == null || Output.View == null)
                return; 

            var ellInput = Input.View.FindDescendent<Ellipse>().FirstOrDefault();
            var ellOutput = Output.View.FindDescendent<Ellipse>().FirstOrDefault();
            if (ellInput == null || ellOutput == null)
                return;

            var canvas = ellInput.FindParent<Canvas>();
            if (canvas == null)
                return;

            Point0 = ellInput.TransformToVisual(canvas).Transform(new Point(ellInput.ActualWidth / 2, ellInput.ActualHeight / 2));
            Point3 = ellOutput.TransformToVisual(canvas).Transform(new Point(ellOutput.ActualWidth / 2, ellOutput.ActualHeight / 2));

        }

        public void SetInput(NodeInterface ni)
        {
            if (Input != null)
            {
                Input.ValueChanged -= Input_ValueChanged;
                Input.Parent.PropertyChanged -= NodeBase_PropertyChanged;
                Input.PropertyChanged -= NodeInterface_PropertyChanged;
            }
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
            }

            Output = ni;
            Output.Parent.PropertyChanged += NodeBase_PropertyChanged;
            Output.PropertyChanged += NodeInterface_PropertyChanged;
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

    }

}
