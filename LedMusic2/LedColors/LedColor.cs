using LedMusic2.Reactive;
using System;

namespace LedMusic2.LedColors
{
    public struct LedColor : ISerializable, IEquatable<LedColor>
    {

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        /// <summary>
        /// Creates a new color with RGB-Parameters
        /// </summary>
        /// <param name="_a">Alpha</param>
        /// <param name="_r">Red</param>
        /// <param name="_g">Green</param>
        /// <param name="_b">Blue</param>
        public LedColor(byte _r, byte _g, byte _b)
        {
            R = _r;
            G = _g;
            B = _b;
        }

        public LedColorHSV GetColorHSV()
        {

            int max = Math.Max(R, Math.Max(G, B));
            int min = Math.Min(R, Math.Min(G, B));

            double hue = System.Drawing.Color.FromArgb(255, R, G, B).GetHue();
            double saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            double value = max / 255d;

            return new LedColorHSV(hue, saturation, value);

        }

        public double GetValue()
        {
            return Math.Max(R, Math.Max(G, B)) / 255.0;
        }

        public static LedColor Parse(string s)
        {

            string[] values = s.Split(',');
            if (values.Length != 3)
                throw new FormatException();

            try
            {
                byte r = byte.Parse(values[0]);
                byte g = byte.Parse(values[1]);
                byte b = byte.Parse(values[2]);
                return new LedColor(r, g, b);
            }
            catch (FormatException)
            {
                throw;
            }

        }

        #region Mixing
        private LedColor apply(Func<byte, byte, float, float, int> f, LedColor other, float factor)
        {
            float inverseFactor = 1.0f - factor;
            int red = f(R, other.R, factor, inverseFactor);
            int green = f(G, other.G, factor, inverseFactor);
            int blue = f(B, other.B, factor, inverseFactor);
            return clamp(red, green, blue);
        }

        private LedColor applyFloat(Func<float, float, float, float, float> f, LedColor other, float factor)
        {
            float inverseFactor = 1.0f - factor;
            var a = LedColorRGBf.FromLedColorRGB(this);
            var b = LedColorRGBf.FromLedColorRGB(other);
            float red = f(a.R, b.R, factor, inverseFactor);
            float green = f(a.G, b.G, factor, inverseFactor);
            float blue = f(a.B, b.B, factor, inverseFactor);
            return clamp(red, green, blue);
        }

        public LedColor Mix(LedColor other, float factor) => apply((a, b, f, i) => (int)(i * a + f * b), other, factor);
        public LedColor Add(LedColor other, float factor) => apply((a, b, f, i) => (int)(a + f * b), other, factor);

        public LedColor Overlay(LedColor other, float factor)
        {
            double alphaA = GetValue();
            byte red = Convert.ToByte(alphaA * R + (1 - alphaA) * other.R);
            byte green = Convert.ToByte(alphaA * G + (1 - alphaA) * other.G);
            byte blue = Convert.ToByte(alphaA * B + (1 - alphaA) * other.B);
            return new LedColor(red, green, blue);
        }

        public LedColor Multiply(LedColor other, float factor) => applyFloat((a, b, f, i) => a * (i + f * b), other, factor);
        public LedColor Subtract(LedColor other, float factor) => applyFloat((a, b, f, i) => a - f * b, other, factor);
        public LedColor Screen(LedColor other, float factor) => applyFloat((a, b, f, i) => 1.0f - (i + f * (1.0f - b)) * (1.0f - a), other, factor);
        public LedColor Divide(LedColor other, float factor) => applyFloat((a, b, f, i) => b != 0.0f ? (i * a + f * a / b) : 0f, other, factor);
        public LedColor Difference(LedColor other, float factor) => applyFloat((a, b, f, i) => i * a + f * Math.Abs(a - b), other, factor);
        public LedColor Darken(LedColor other, float factor) => applyFloat((a, b, f, i) => Math.Min(a, b) * f + a * i, other, factor);
        public LedColor Lighten(LedColor other, float factor) => applyFloat((a, b, f, i) => Math.Max(f * b, a), other, factor);
        #endregion

        #region Clamping
        private LedColor clamp(float r, float g, float b)
        {
            return new LedColor(
                (byte)Math.Max(0, Math.Min(255, r * 255)),
                (byte)Math.Max(0, Math.Min(255, g * 255)),
                (byte)Math.Max(0, Math.Min(255, b * 255)));
        }

        private LedColor clamp(int r, int g, int b)
        {
            return new LedColor(
                (byte)Math.Max(0, Math.Min(255, r)),
                (byte)Math.Max(0, Math.Min(255, g)),
                (byte)Math.Max(0, Math.Min(255, b)));
        }
        #endregion

        public string Serialize() => Convert.ToBase64String(new byte[] { R, G, B });

        public object Deserialize(string s)
        {
            var bytes = Convert.FromBase64String(s);
            return new LedColor(bytes[0], bytes[1], bytes[2]);
        }

        public override string ToString() => string.Format("{0},{1},{2}", R, G, B);
        public bool Equals(LedColor obj) => R == obj.R && G == obj.G && B == obj.B;
        public override int GetHashCode() => R ^ G ^ B;

    }
}
