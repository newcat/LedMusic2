using System;

namespace LedMusic2.LedColors
{
    public abstract class LedColor
    {

        public abstract LedColorRGB GetColorRGB();
        public abstract LedColorHSV GetColorHSV();

        public override string ToString()
        {
            LedColorRGB c = GetColorRGB();
            return string.Format("{0},{1},{2}", c.R, c.G, c.B);
        }

        #region Mixing
        public LedColor Mix(LedColor other, double factor)
        {

            var a = GetColorRGB();
            var b = other.GetColorRGB();

            double factorm = 1.0 - factor;
            int red = (int)(factorm * a.R + factor * b.R);
            int green = (int)(factorm * a.G + factor * b.G);
            int blue = (int)(factorm * a.B + factor * b.B);
            return Clamp(red, green, blue);

        }

        public LedColor Add(LedColor other, double factor)
        {

            var a = GetColorRGB();
            var b = other.GetColorRGB();

            int red = (int)(a.R + factor * b.R);
            int green = (int)(a.G + factor * b.G);
            int blue = (int)(a.B + factor * b.B);
            return Clamp(red, green, blue);

        }

        public LedColor Overlay(LedColor other, float factor)
        {

            var a = GetColorRGB();
            var b = other.GetColorRGB();

            double alphaA = a.GetValue();
            byte red = Convert.ToByte(alphaA * a.R + (1 - alphaA) * b.R);
            byte green = Convert.ToByte(alphaA * a.G + (1 - alphaA) * b.G);
            byte blue = Convert.ToByte(alphaA * a.B + (1 - alphaA) * b.B);
            return new LedColorRGB(red, green, blue);

        }

        public LedColor Multiply(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float facm = 1.0f - factor;
            float red = a.R * (facm + factor * b.R);
            float green = a.G * (facm + factor * b.G);
            float blue = a.B * (facm + factor * b.B);
            return Clamp(red, green, blue);

        }

        public LedColor Subtract(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float red = a.R - factor * b.R;
            float green = a.G - factor * b.G;
            float blue = a.B - factor * b.B;
            return Clamp(red, green, blue);

        }

        public LedColor Screen(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float facm = 1.0f - factor;
            float red = 1.0f - (facm + factor * (1.0f - b.R)) * (1.0f - a.R);
            float green = 1.0f - (facm + factor * (1.0f - b.G)) * (1.0f - a.G);
            float blue = 1.0f - (facm + factor * (1.0f - b.B)) * (1.0f - a.B);
            return Clamp(red, green, blue);

        }

        public LedColor Divide(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float facm = 1.0f - factor;
            float red = b.R != 0.0f ? (facm * a.R + factor * a.R / b.R) : 0f;
            float green = b.G != 0.0f ? (facm * a.G + factor * a.G / b.G) : 0f;
            float blue = b.B != 0.0f ? (facm * a.B + factor * a.B / b.B) : 0f;
            return Clamp(red, green, blue);

        }

        public LedColor Difference(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float facm = 1.0f - factor;
            float red = facm * a.R + factor * Math.Abs(a.R - b.R);
            float green = facm * a.G + factor * Math.Abs(a.G - b.G);
            float blue = facm * a.B + factor * Math.Abs(a.B - b.B);
            return Clamp(red, green, blue);

        }

        public LedColor Darken(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float facm = 1.0f - factor;
            float red = Math.Min(a.R, b.R) * factor + a.R * facm;
            float green = Math.Min(a.G, b.G) * factor + a.G * facm;
            float blue = Math.Min(a.B, b.B) * factor + a.B * facm;
            return Clamp(red, green, blue);

        }

        public LedColor Lighten(LedColor other, float factor)
        {

            var a = LedColorRGBf.FromLedColorRGB(GetColorRGB());
            var b = LedColorRGBf.FromLedColorRGB(other.GetColorRGB());

            float red = Math.Max(factor * b.R, a.R);
            float green = Math.Max(factor * b.G, a.G);
            float blue = Math.Max(factor * b.B, a.B);
            return Clamp(red, green, blue);

        }
        #endregion

        #region Clamping
        private float Clamp(float f)
        {
            return Math.Max(0, Math.Min(1, f));
        }

        private LedColorRGB Clamp(float r, float g, float b)
        {
            return new LedColorRGB(
                (byte)Math.Max(0, Math.Min(255, r * 255)),
                (byte)Math.Max(0, Math.Min(255, g * 255)),
                (byte)Math.Max(0, Math.Min(255, b * 255)));
        }

        private LedColorRGB Clamp(int r, int g, int b)
        {
            return new LedColorRGB(
                (byte)Math.Max(0, Math.Min(255, r)),
                (byte)Math.Max(0, Math.Min(255, g)),
                (byte)Math.Max(0, Math.Min(255, b)));
        }
        #endregion

    }

}