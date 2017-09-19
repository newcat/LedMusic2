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
            optMode.DisplayValue = "Overlay";
            _options.Add(optMode);

            Calculate();

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

                LedColorRGBf a = i < colorsA.Length ?
                    (colorsA[i] != null ? LedColorRGBf.FromLedColorRGB(colorsA[i].GetColorRGB()) :
                    new LedColorRGBf(0f, 0f, 0f)) : new LedColorRGBf(0f, 0f, 0f);

                LedColorRGBf b = i < colorsB.Length ?
                    (colorsB[i] != null ? LedColorRGBf.FromLedColorRGB(colorsB[i].GetColorRGB()) :
                    new LedColorRGBf(0f, 0f, 0f)) : new LedColorRGBf(0f, 0f, 0f);

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
                    case "Screen":
                        result[i] = screen(a, b);
                        break;
                    case "Divide":
                        result[i] = divide(a, b);
                        break;
                    case "Difference":
                        result[i] = difference(a, b);
                        break;
                    case "Darken":
                        result[i] = darken(a, b);
                        break;
                    case "Lighten":
                        result[i] = lighten(a, b);
                        break;
                    case "Overlay":
                        result[i] = overlay(a, b);
                        break;

                    default:
                        result[i] = overlay(a, b);
                        break;
                }

            }

            _outputs[0].SetValue(result);
            return true;

        }

        private LedColorRGB mix(LedColorRGBf a, LedColorRGBf b)
        {
            float factorm = 1.0f - factor;
            float red = factorm * a.R + factor * b.R;
            float green = factorm * a.G + factor * b.G;
            float blue = factorm * a.B + factor * b.B;
            return clamp(red, green, blue);
        }

        private LedColorRGB add(LedColorRGBf a, LedColorRGBf b)
        {
            float red = a.R + factor * b.R;
            float green = a.G + factor * b.G;
            float blue = a.B + factor * b.B;
            return clamp(red, green, blue);
        }

        private LedColorRGB multiply(LedColorRGBf a, LedColorRGBf b)
        {
            float facm = 1.0f - factor;
            float red = a.R * (facm + factor * b.R);
            float green = a.G * (facm + factor * b.G);
            float blue = a.B * (facm + factor * b.B);
            return clamp(red, green, blue);
        }

        private LedColorRGB subtract(LedColorRGBf a, LedColorRGBf b)
        {
            float red = a.R - factor * b.R;
            float green = a.G - factor * b.G;
            float blue = a.B - factor * b.B;
            return clamp(red, green, blue);
        }

        private LedColorRGB screen(LedColorRGBf a, LedColorRGBf b)
        {
            float facm = 1.0f - factor;
            float red = 1.0f - (facm + factor * (1.0f - b.R)) * (1.0f - a.R);
            float green = 1.0f - (facm + factor * (1.0f - b.G)) * (1.0f - a.G);
            float blue = 1.0f - (facm + factor * (1.0f - b.B)) * (1.0f - a.B);
            return clamp(red, green, blue);
        }

        private LedColorRGB divide(LedColorRGBf a, LedColorRGBf b)
        {
            float facm = 1.0f - factor;
            float red = b.R != 0.0f ? (facm * a.R + factor * a.R / b.R) : 0f;
            float green = b.G != 0.0f ? (facm * a.G + factor * a.G / b.G) : 0f;
            float blue = b.B != 0.0f ? (facm * a.B + factor * a.B / b.B) : 0f;
            return clamp(red, green, blue);
        }

        private LedColorRGB difference(LedColorRGBf a, LedColorRGBf b)
        {
            float facm = 1.0f - factor;
            float red = facm * a.R + factor * Math.Abs(a.R - b.R);
            float green = facm * a.G + factor * Math.Abs(a.G - b.G);
            float blue = facm * a.B + factor * Math.Abs(a.B - b.B);
            return clamp(red, green, blue);
        }

        private LedColorRGB darken(LedColorRGBf a, LedColorRGBf b)
        {
            float facm = 1.0f - factor;
            float red = Math.Min(a.R, b.R) * factor + a.R * facm;
            float green = Math.Min(a.G, b.G) * factor + a.G * facm;
            float blue = Math.Min(a.B, b.B) * factor + a.B * facm;
            return clamp(red, green, blue);
        }

        private LedColorRGB lighten(LedColorRGBf a, LedColorRGBf b)
        {
            float red = Math.Max(factor * b.R, a.R);
            float green = Math.Max(factor * b.G, a.G);
            float blue = Math.Max(factor * b.B, a.B);
            return clamp(red, green, blue);
        }

        private LedColorRGB overlay(LedColorRGBf a, LedColorRGBf b)
        {
            return a.GetColorRGB().Overlay(b.GetColorRGB());
        }

        private float clamp(float f)
        {
            return Math.Max(0, Math.Min(1, f));
        }

        private LedColorRGB clamp(float r, float g, float b)
        {
            return new LedColorRGBf(clamp(r), clamp(g), clamp(b)).GetColorRGB();
        }

        private float getValue(LedColorRGBf c)
        {
            return Math.Max(Math.Max(c.R, c.G), c.B);
        }

        private class LedColorRGBf : LedColor
        {

            public static LedColorRGBf FromLedColorRGB(LedColorRGB c)
            {
                return new LedColorRGBf(c.R / 255f, c.G / 255f, c.B / 255f);
            }

            public float R { get; set; }
            public float G { get; set; }
            public float B { get; set; }

            public LedColorRGBf(float r, float g, float b)
            {
                R = r;
                G = g;
                B = b;
            }

            public override LedColorRGB GetColorRGB()
            {
                byte red = (byte)(255 * R);
                byte green = (byte)(255 * G);
                byte blue = (byte)(255 * B);
                return new LedColorRGB(red, green, blue);
            }

            public override LedColorHSV GetColorHSV()
            {
                return GetColorRGB().GetColorHSV();
            }

        }

    }
}
