using AttachedCommandBehavior;
using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Nodes.NodeViews;
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
using System.Windows.Forms;
using System.Windows.Media;

namespace LedMusic2.Nodes
{

    [Node("Color Ramp", NodeCategory.COLOR)]
    public class ColorRampNode : NodeBase
    {

        #region ViewModel Properties
        private LinearGradientBrush _fillBrush = new LinearGradientBrush(Colors.Black, Colors.White, 0);
        public LinearGradientBrush FillBrush
        {
            get {
                return _fillBrush;
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

        private ObservableCollection<ColorStopViewModel> _colorStops = new ObservableCollection<ColorStopViewModel>();
        public ObservableCollection<ColorStopViewModel> ColorStops
        {
            get { return _colorStops; }
            set
            {
                _colorStops = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("FillBrush");
            }
        }

        private ColorStopViewModel _selectedColorStop = null;
        public ColorStopViewModel SelectedColorStop
        {
            get { return _selectedColorStop; }
            set
            {
                _selectedColorStop = value;
                NotifyPropertyChanged();
            }
        }

        private SimpleCommand _cmdAddStop = new SimpleCommand();
        public SimpleCommand CmdAddStop { get { return _cmdAddStop; } }

        private SimpleCommand _cmdRemoveStop = new SimpleCommand();
        public SimpleCommand CmdRemoveStop { get { return _cmdRemoveStop; } }

        public SimpleCommand CmdSelectColor
        {
            get { return new SimpleCommand() { ExecuteDelegate = (o) => setColorOfSelected() }; }
        }
        #endregion

        public ColorRampNode(Point initPosition) : base(initPosition)
        {

            _outputs.Add(new NodeInterface<LedColor[]>("Color", ConnectionType.COLOR_ARRAY, this, false));

            _options.Add(new NodeOptionViewModel(NodeOptionType.CUSTOM, "Test", typeof(NodeViews.ColorRampNode), this));

            _cmdAddStop.ExecuteDelegate = (o) => _colorStops.Add(new ColorStopViewModel(Colors.White, 0.0, this));

            _cmdRemoveStop.CanExecuteDelegate = (o) => SelectedColorStop != null;
            _cmdRemoveStop.ExecuteDelegate = (o) =>
            {
                if (SelectedColorStop != null)
                {
                    _colorStops.Remove(SelectedColorStop);
                    SelectedColorStop.Dispose();
                    SelectedColorStop = null;
                }
            };


        }

        public override bool Calculate()
        {
            return true;
        }

        public void SelectStop(ColorStopViewModel vm)
        {
            if (SelectedColorStop != null)
                SelectedColorStop.IsSelected = false;
            vm.IsSelected = true;
            SelectedColorStop = vm;
        }

        private void setColorOfSelected()
        {
            var c = SelectedColorStop.Color;
            var dlg = new ColorDialog();
            dlg.Color = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var resultColor = dlg.Color;
                SelectedColorStop.Color = Color.FromRgb(resultColor.R, resultColor.G, resultColor.B);
            }
            dlg.Dispose();
        }

    }

}
