using LedMusic2.Attributes;
using LedMusic2.Enums;
using LedMusic2.Models;
using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LedMusic2.Nodes
{
    [Node("Mix Color", NodeCategory.COLOR)]
    class MixColorNode : NodeBase
    {

        private float factor = 0.5f;
        private NodeOptionViewModel optMode;

        public MixColorNode(Point initPosition) : base(initPosition)
        {

            MinWidth = 170;

            _inputs.Add(new NodeInterface<double>("Factor", ConnectionType.NUMBER, this, true));
            _inputs.Add(new NodeInterface<LedColor[]>("Color 1", ConnectionType.COLOR_ARRAY, this, true));
            _inputs.Add(new NodeInterface<LedColor[]>("Color 2", ConnectionType.COLOR_ARRAY, this, true));

            _outputs.Add(new NodeInterface<LedColor[]>("Output", ConnectionType.COLOR_ARRAY, this, false));

            optMode = new NodeOptionViewModel(NodeOptionType.SELECTION, "Mode");
            foreach (string s in new string[] { "Mix", "Add", "Multiply", "Subtract", "Screen", "Divide", "Difference", "Darken", "Lighten",
                                                "Overlay", "Dodge", "Burn", "Hue", "Saturation", "Value", "Color", "Soft Light", "Linear Light"})
            {
                optMode.Options.Add(s);
            }
            optMode.DisplayValue = "Mix";
            optMode.PropertyChanged += OptMode_PropertyChanged;
            _options.Add(optMode);

            Calculate();

        }

        private void OptMode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                InvokeOutputChanged();
        }

        public override bool Calculate()
        {

            LedColor[] colorsA = (LedColor[])_inputs.GetNodeInterface("Color 1").GetValue();
            LedColor[] colorsB = (LedColor[])_inputs.GetNodeInterface("Color 2").GetValue();

            if (colorsA == null || colorsB == null)
                return false;

            factor = (float)(double)_inputs.GetNodeInterface("Factor").GetValue();
            string mode = (string)optMode.RenderValue;

            int length = Math.Max(colorsA.Length, colorsB.Length);
            LedColorRGB[] result = new LedColorRGB[length];

            for (int i = 0; i < length; i++)
            {

                LedColorRGB a = i < colorsA.Length ?
                    (colorsA[i] != null ? colorsA[i].getColorRGB() :
                    new LedColorRGB(0, 0, 0)) : new LedColorRGB(0, 0, 0);

                LedColorRGB b = i < colorsB.Length ?
                    (colorsB[i] != null ? colorsB[i].getColorRGB() :
                    new LedColorRGB(0, 0, 0)) : new LedColorRGB(0, 0, 0);

                switch (mode)
                {
                    case "Mix":
                        result[i] = mix(a, b);
                        break;
                    case "Add":
                        result[i] = add(a, b);
                        break;
                    case "Multiply":
                        result[i] = multiply(a, b);
                        break;
                    case "Subtract":
                        result[i] = subtract(a, b);
                        break;

                    default:
                        result[i] = mix(a, b);
                        break;
                }

            }

            _outputs[0].SetValue(result);
            return true;

        }

        private LedColorRGB mix(LedColorRGB a, LedColorRGB b)
        {
            byte red = (byte)((1.0f - factor) * (a.R) + factor * (b.R));
            byte green = (byte)((1.0f - factor) * (a.G) + factor * (b.G));
            byte blue = (byte)((1.0f - factor) * (a.B) + factor * (b.B));
            return new LedColorRGB(red, green, blue);
        }

        private LedColorRGB add(LedColorRGB a, LedColorRGB b)
        {
            int red = (int)(a.R + factor * b.R);
            int green = (int)(a.G + factor * b.G);
            int blue = (int)(a.B + factor * b.B);
            return clamp(red, green, blue);
        }

        private LedColorRGB multiply(LedColorRGB a, LedColorRGB b)
        {
            int red = 0;
            if (b.R >= 128)
                red = (int)(a.R + factor * (2.0f * (b.R - 0.5f)));
            else
                red = (int)(a.R + factor * (2.0f * (b.R - 1.0f)));

            int green = 0;
            if (b.G >= 128)
                green = (int)(a.G + factor * (2.0f * (b.G - 0.5f)));
            else
                green = (int)(a.G + factor * (2.0f * (b.G - 1.0f)));

            int blue = 0;
            if (b.B >= 128)
                blue = (int)(a.B + factor * (2.0f * (b.B - 0.5f)));
            else
                blue = (int)(a.B + factor * (2.0f * (b.B - 1.0f)));

            return clamp(red, green, blue);
        }

        private LedColorRGB subtract(LedColorRGB a, LedColorRGB b)
        {
            int red = (int)(a.R - factor * b.R);
            int green = (int)(a.G - factor * b.G);
            int blue = (int)(a.B - factor * b.B);
            return clamp(red, green, blue);
        }

        private byte clamp(int b)
        {
            return (byte)Math.Max(0, Math.Min(255, b));
        }

        private LedColorRGB clamp(int r, int g, int b)
        {
            return new LedColorRGB(clamp(r), clamp(g), clamp(b));
        }

    }
}
