using System;

namespace LedMusic2.LedColors
{
    public class LedColorRGB : LedColor
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
        public LedColorRGB(byte _r, byte _g, byte _b)
        {
            R = _r;
            G = _g;
            B = _b;
        }

        public override LedColorRGB GetColorRGB() { return this; }

        public override LedColorHSV GetColorHSV()
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

        public static LedColorRGB Parse(string s)
        {

            string[] values = s.Split(',');
            if (values.Length != 3)
                throw new FormatException();

            try
            {
                byte r = byte.Parse(values[0]);
                byte g = byte.Parse(values[1]);
                byte b = byte.Parse(values[2]);
                return new LedColorRGB(r, g, b);
            }
            catch (FormatException)
            {
                throw;
            }

        }

        protected override void SetRGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

    }
}
