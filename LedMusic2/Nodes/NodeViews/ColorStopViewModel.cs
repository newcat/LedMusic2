using AttachedCommandBehavior;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LedMusic2.Nodes.NodeViews
{
    public class ColorStopViewModel : INotifyPropertyChanged, IDisposable
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Action<object> DragDeltaAction
        {
            get { return new Action<object>((e) => addToPosition(((DragDeltaEventArgs)e).HorizontalChange)); }
        }

        public SimpleCommand CmdLeftClick
        {
            get { return new SimpleCommand() { ExecuteDelegate = (o) => parent?.SelectStop(this) }; }
        }

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
            get { return Position * parent.Width - 5; }
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

        private Nodes.ColorRampNode parent;

        public ColorStopViewModel(Color c, double p, Nodes.ColorRampNode n)
        {
            Color = c;
            Position = p;
            parent = n;
            parent.PropertyChanged += Parent_PropertyChanged;
        }

        private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Width")
                NotifyPropertyChanged("AbsolutePosition");
        }

        private void addToPosition(double px)
        {
            Position = Math.Max(0, Math.Min(1, Position + px / parent.Width));
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (parent != null)
                        parent.PropertyChanged -= Parent_PropertyChanged;
                    parent = null;
                }

                disposedValue = true;
            }
        }

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~ColorStopViewModel() {
        //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        //   Dispose(false);
        // }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
