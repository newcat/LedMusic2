using AttachedCommandBehavior;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;
using System.Linq;
using LedMusic2.LedColors;
using LedMusic2.NodeEditor;
using LedMusic2.NodeConnection;

namespace LedMusic2.Nodes.NodeModels
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
            get { return new SimpleCommand() { ExecuteDelegate = (o) => SetColorOfSelected() }; }
        }
        #endregion

        public ColorRampNode(Point initPosition, NodeEditorViewModel parentVM) : base(initPosition, parentVM)
        {

            AddInput("Factor", ConnectionType.NUMBER);
            AddOutput<LedColors.LedColor[]>("Color Band");
            AddOutput<LedColors.LedColor>("Single Color");

            Options.Add(new NodeOption(NodeOptionType.CUSTOM, "Test", typeof(NodeViews.ColorRampNode), this));

            CmdAddStop.ExecuteDelegate = (o) => {
                var c = new ColorStopViewModel(Colors.White, 0.0, this);
                _colorStops.Add(c);
                SelectStop(c);
                CalcPreview();
            };

            CmdRemoveStop.CanExecuteDelegate = (o) => SelectedColorStop != null;
            CmdRemoveStop.ExecuteDelegate = (o) =>
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

            for (int i = 0; i < ledCount; i++)
            {

                var currentPos = (double)i / ledCount;
                output[i] = GetColorAtPosition(currentPos);

            }

            Outputs.GetNodeInterface("Color Band").SetValue(output);

            var fac = Math.Max(0.0, Math.Min(1.0, ((NodeInterface<double>)Inputs.GetNodeInterface("Factor")).Value));
            Outputs.GetNodeInterface("Single Color").SetValue(GetColorAtPosition(fac));

            return true;

        }

        private LedColorRGB GetColorAtPosition(double position)
        {

            var prev = ColorStops.LastOrDefault(x => x.Position <= position);
            var next = ColorStops.FirstOrDefault(x => x.Position > position);

            if (prev == null && next == null)
                return null;
            else if (prev == null)
                return ColorToLedColor(next.Color);
            else if (next == null || prev.Position == position)
                return ColorToLedColor(prev.Color);
            else
            {
                var fac = (position - prev.Position) / (next.Position - prev.Position);
                return new LedColorRGB(
                        (byte)Math.Max(0, Math.Min(255, prev.Color.R + fac * (next.Color.R - prev.Color.R))),
                        (byte)Math.Max(0, Math.Min(255, prev.Color.G + fac * (next.Color.G - prev.Color.G))),
                        (byte)Math.Max(0, Math.Min(255, prev.Color.B + fac * (next.Color.B - prev.Color.B))));
            }

        }

        private LedColorRGB ColorToLedColor(Color c)
        {
            return new LedColorRGB(c.R, c.G, c.B);
        }

        public void SelectStop(ColorStopViewModel vm)
        {
            if (SelectedColorStop != null)
                SelectedColorStop.IsSelected = false;
            vm.IsSelected = true;
            SelectedColorStop = vm;
        }

        private void SetColorOfSelected()
        {
            var c = SelectedColorStop.Color;
            var dlg = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(255, c.R, c.G, c.B)
            };
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
                        LedColorRGB c = LedColorRGB.Parse(colorStopX.Value);
                        ColorStopViewModel cvm = new ColorStopViewModel(
                            Color.FromRgb(c.R, c.G, c.B), pos, this);
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
