using AttachedCommandBehavior;
using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.Nodes.NodeViews;
using LedMusic2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;

namespace LedMusic2.Nodes
{

    [Node("Color Ramp", NodeCategory.COLOR)]
    public class ColorRampNode : NodeBase
    {

        #region ViewModel Properties
        private LinearGradientBrush _fillBrush = new LinearGradientBrush(Colors.Black, Colors.White, 0);
        public LinearGradientBrush FillBrush
        {
            get { return _fillBrush; }
            set
            {
                _fillBrush = value;
                NotifyPropertyChanged();
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

            _cmdAddStop.ExecuteDelegate = (o) => {
                var c = new ColorStopViewModel(Colors.White, 0.0, this);
                _colorStops.Add(c);
                SelectStop(c);
                CalcPreview();
            };

            _cmdRemoveStop.CanExecuteDelegate = (o) => SelectedColorStop != null;
            _cmdRemoveStop.ExecuteDelegate = (o) =>
            {
                if (SelectedColorStop != null)
                {
                    _colorStops.Remove(SelectedColorStop);
                    SelectedColorStop.Dispose();
                    SelectedColorStop = null;
                    CalcPreview();
                }
            };

            ColorStops.Add(new ColorStopViewModel(Colors.Black, 0, this));
            ColorStops.Add(new ColorStopViewModel(Colors.White, 1, this));

        }

        public override bool Calculate()
        {

            var ledCount = GlobalProperties.Instance.LedCount;
            var output = new LedColorRGB[ledCount];

            if (ColorStops.Count == 0)
            {
                for (int i = 0; i < ledCount; i++)
                {
                    output[i] = new LedColorRGB(0, 0, 0);
                }
                _outputs[0].SetValue(output);
                return true;
            }

            ColorStops.Sort();

            var currentStopIndex = 0;
            var colorStopCount = ColorStops.Count;

            for (int i = 0; i < ledCount; i++)
            {

                var currentPos = (double)i / ledCount;
                if (currentStopIndex < colorStopCount - 1 && currentPos > ColorStops[currentStopIndex + 1].Position)
                {
                    currentStopIndex++;
                }

                if (colorStopCount - 1 == currentStopIndex)
                {
                    var c = ColorStops[currentStopIndex].Color;
                    output[i] = new LedColorRGB(c.R, c.G, c.B);
                } else
                {
                    
                    var stop1 = ColorStops[currentStopIndex];
                    var stop2 = ColorStops[currentStopIndex + 1];
                    var fac = 0.5;

                    if (currentPos <= stop1.Position)
                        fac = 0;
                    else if (currentPos >= stop2.Position)
                        fac = 1;
                    else
                        fac = (currentPos - stop1.Position) / (stop2.Position - stop1.Position);

                    output[i] = new LedColorRGB(
                        (byte)Math.Min(255, stop1.Color.R + fac * (stop2.Color.R - stop1.Color.R)),
                        (byte)Math.Min(255, stop1.Color.G + fac * (stop2.Color.G - stop1.Color.G)),
                        (byte)Math.Min(255, stop1.Color.B + fac * (stop2.Color.B - stop1.Color.B))
                        );

                }

            }

            _outputs[0].SetValue(output);

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
                CalcPreview();
            }
            dlg.Dispose();
        }

        public void CalcPreview()
        {

            var sc = new GradientStopCollection();
            foreach (ColorStopViewModel s in ColorStops)
            {
                sc.Add(new GradientStop(s.Color, s.Position));
            }
            FillBrush = new LinearGradientBrush(sc, 0);

            InvokeOutputChanged();

        }

        protected override void LoadAdditionalXmlData(XElement x)
        {
            ColorStops.Clear();
            SelectedColorStop = null;
            foreach (XElement el in x.Elements())
            {
                if (el.Name.LocalName == "colorstops")
                {
                    foreach (XElement colorStopX in el.Elements())
                    {
                        double pos = double.Parse(colorStopX.Attribute("position").Value, CultureInfo.InvariantCulture);
                        LedColorRGB c = LedColor.Parse(colorStopX.Value);
                        ColorStopViewModel cvm = new ColorStopViewModel(Color.FromRgb(c.R, c.G, c.B), pos, this);
                        ColorStops.Add(cvm);
                    }
                }
            }
            CalcPreview();
        }

        protected override void SaveAdditionalXmlData(XElement x)
        {
            XElement colorStopsX = new XElement("colorstops");
            foreach (ColorStopViewModel cvm in ColorStops)
            {
                Color c = cvm.Color;
                XElement colorStopX = new XElement("colorstop", new LedColorRGB(c.R, c.G, c.B));
                colorStopX.SetAttributeValue("position", cvm.Position);
                colorStopsX.Add(colorStopX);
            }
            x.Add(colorStopsX);
        }

    }

}
