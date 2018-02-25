using LedMusic2.NodeEditor;
using LedMusic2.Nodes;
using LedMusic2.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace LedMusic2.NodeConnection
{
    public class TemporaryConnection : VMBase, IDisposable
    {

        public NodeInterface OriginInterface { get; private set; }
        public NodeInterface TargetInterface { get; private set; }

        private bool _isConnectionAllowed = false;
        public bool IsConnectionAllowed
        {
            get { return _isConnectionAllowed; }
            set
            {
                _isConnectionAllowed = value;
                NotifyPropertyChanged();
            }
        }

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
            get { return new Point(Point3.X - (Point3.X - Point0.X) / 2, Point3.Y); }
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

        public int ZIndex { get { return 3; } }

        private NodeEditorViewModel nodeEditorVM;

        public TemporaryConnection(NodeInterface originInterface, NodeEditorViewModel parentVM)
        {
            nodeEditorVM = parentVM;
            nodeEditorVM.PropertyChanged += NodeEditorVM_PropertyChanged;
            SetOriginInterface(originInterface);
            Point3 = Point0;
        }

        private void SetOriginInterface(NodeInterface oi)
        {
            OriginInterface = oi;
            if (oi != null && oi.View != null)
            {
                var ell = oi.View.FindDescendent<Ellipse>().FirstOrDefault();
                if (ell == null)
                    return;

                var canvas = ell.FindParent<Canvas>();
                if (canvas == null)
                    return;

                Point0 = ell.TransformToVisual(canvas).Transform(new Point(ell.ActualWidth / 2, ell.ActualHeight / 2));
            }
        }

        public void SetTargetInterface(NodeInterface ti)
        {
            TargetInterface = ti;
            if (ti != null && ti.View != null)
            {
                var ell = ti.View.FindDescendent<Ellipse>().FirstOrDefault();
                if (ell == null)
                    return;

                var canvas = ell.FindParent<Canvas>();
                if (canvas == null)
                    return;

                Point3 = ell.TransformToVisual(canvas).Transform(new Point(ell.ActualWidth / 2, ell.ActualHeight / 2));
            }
        }

        private void NodeEditorVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (TargetInterface == null && (e.PropertyName == "MousePosX" || e.PropertyName == "MousePosY"))
                Point3 = new Point(nodeEditorVM.MousePosX, nodeEditorVM.MousePosY);
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
                    nodeEditorVM.PropertyChanged -= NodeEditorVM_PropertyChanged;
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
