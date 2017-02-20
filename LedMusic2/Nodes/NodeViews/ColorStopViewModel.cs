using LedMusic2.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace LedMusic2.Nodes.NodeViews
{
    public class ColorStopViewModel : VMBase, IDisposable, IComparable<ColorStopViewModel>
    {

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        private double _position = 0;
        public double Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("AbsolutePosition");
            }
        }

        public double AbsolutePosition
        {
            get { return Position * Parent.Width - 5; }
        }

        private Color _color = Colors.Red;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyPropertyChanged();
            }
        }

        public Nodes.ColorRampNode Parent { get; private set; }

        public ColorStopViewModel(Color c, double p, Nodes.ColorRampNode n)
        {
            Color = c;
            Position = p;
            Parent = n;
            Parent.PropertyChanged += Parent_PropertyChanged;
        }

        private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Width")
                NotifyPropertyChanged("AbsolutePosition");
        }

        public void AddToPosition(double px)
        {
            if (Parent.Width > 0)
            {
                Position = Math.Max(0, Math.Min(1, Position + px / Parent.Width));
                Parent.CalcPreview();
            }
        }

        public int CompareTo(ColorStopViewModel other)
        {
            if (Position == other.Position)
                return 0;
            else
                return Position > other.Position ? 1 : -1;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Parent != null)
                        Parent.PropertyChanged -= Parent_PropertyChanged;
                    Parent = null;
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
