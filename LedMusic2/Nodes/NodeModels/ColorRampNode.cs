using LedMusic2.LedColors;
using LedMusic2.NodeConnection;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace LedMusic2.Nodes.NodeModels
{

    [Node("Color Ramp", NodeCategory.COLOR)]
    public class ColorRampNode : NodeBase
    {

        public double Width { get; set; } = 0;
        public ObservableCollection<ColorStopViewModel> ColorStops { get; } = new ObservableCollection<ColorStopViewModel>();
        public ColorStopViewModel SelectedColorStop { get; set; } = null;

        public ColorRampNode() : base()
        {

            AddInput("Factor", 0.5);
            AddOutput<LedColor[]>("Color Band");
            AddOutput<LedColor>("Single Color");

            //Options.Add(new NodeOption(NodeOptionType.CUSTOM, "Test", typeof(NodeViews.ColorRampNode), this));

            ColorStops.Add(new ColorStopViewModel(new LedColorRGB(0, 0, 0), 0));
            ColorStops.Add(new ColorStopViewModel(new LedColorRGB(255, 255, 255), 1));

        }

        public override bool Calculate()
        {

            var resolution = GlobalProperties.Instance.Resolution;
            var output = new LedColorRGB[resolution];

            if (ColorStops.Count == 0)
            {
                for (int i = 0; i < resolution; i++)
                {
                    output[i] = new LedColorRGB(0, 0, 0);
                }
                Outputs[0].SetValue(output);
                return true;
            }

            ColorStops.Sort();

            for (int i = 0; i < resolution; i++)
            {

                var currentPos = (double)i / resolution;
                output[i] = getColorAtPosition(currentPos);

            }

            Outputs.GetNodeInterface("Color Band").SetValue(output);

            var fac = Math.Max(0.0, Math.Min(1.0, ((NodeInterface<double>)Inputs.GetNodeInterface("Factor")).Value));
            Outputs.GetNodeInterface("Single Color").SetValue(getColorAtPosition(fac));

            return true;

        }

        private LedColorRGB getColorAtPosition(double position)
        {

            var prev = ColorStops.LastOrDefault(x => x.Position <= position);
            var next = ColorStops.FirstOrDefault(x => x.Position > position);

            if (prev == null && next == null)
                return null;
            else if (prev == null)
                return next.Color;
            else if (next == null || prev.Position == position)
                return prev.Color;
            else
            {
                var fac = (position - prev.Position) / (next.Position - prev.Position);
                return new LedColorRGB(
                        (byte)Math.Max(0, Math.Min(255, prev.Color.R + fac * (next.Color.R - prev.Color.R))),
                        (byte)Math.Max(0, Math.Min(255, prev.Color.G + fac * (next.Color.G - prev.Color.G))),
                        (byte)Math.Max(0, Math.Min(255, prev.Color.B + fac * (next.Color.B - prev.Color.B))));
            }

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
                        ColorStopViewModel cvm = new ColorStopViewModel(c, pos);
                        ColorStops.Add(cvm);
                    }
                }
            }
        }

        protected override void SaveAdditionalXmlData(XElement x)
        {
            XElement colorStopsX = new XElement("colorstops");
            foreach (ColorStopViewModel cvm in ColorStops)
            {
                XElement colorStopX = new XElement("colorstop", cvm.Color);
                colorStopX.SetAttributeValue("position", cvm.Position);
                colorStopsX.Add(colorStopX);
            }
            x.Add(colorStopsX);
        }

    }

}
