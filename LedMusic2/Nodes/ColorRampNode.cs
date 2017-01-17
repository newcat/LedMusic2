using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LedMusic2.Nodes
{

    [Node("Color Ramp", NodeCategory.COLOR)]
    class ColorRampNode : NodeBase
    {

        #region ViewModel Properties
        private LinearGradientBrush _fillBrush = new LinearGradientBrush(Colors.Black, Colors.White, 0);
        public LinearGradientBrush FillBrush
        {
            get {
                //TODO
            }
        }

        private double _width = 0;
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<CustomColorStop> _colorStops = new ObservableCollection<CustomColorStop>();
        public ObservableCollection<CustomColorStop> ColorStops
        {
            get { return _colorStops; }
            set
            {
                _colorStops = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("FillBrush");
            }
        }
        #endregion

        public ColorRampNode(Point initPosition) : base(initPosition)
        {

            _outputs.Add(new NodeInterface<LedColor[]>("Color", ConnectionType.COLOR_ARRAY, this, false));

            _options.Add(new NodeOptionViewModel(NodeOptionType.CUSTOM, "Test", typeof(NodeViews.ColorRampNode), this));

            _colorStops.Add(new GradientStop(Colors.White, 0.0));

        }

        public override bool Calculate()
        {
            return true;
        }

        public class CustomColorStop : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged([CallerMemberName] string name = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            private double _position = 0;
            public double Position
            {
                get { return _position; }
                set
                {
                    _position = value;
                    NotifyPropertyChanged();
                }
            }

            private Color _color = Colors.Black;
            public Color Color
            {
                get { return _color; }
                set
                {
                    _color = value;
                    NotifyPropertyChanged();
                }
            }

            public ColorRampNode Parent { get; private set; }

            public CustomColorStop(Color c, double p, ColorRampNode n)
            {
                Color = c;
                Position = p;
                Parent = n;
            }

        }

    }
}
