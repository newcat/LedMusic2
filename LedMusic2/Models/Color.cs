using System;

namespace LedMusic2.Models
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
            } catch (FormatException)
            {
                throw;
            }

        }

    }

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

    }

    public class LedColorHSV : LedColor
    {

        public double H { get; set; } //Range 0..359
        public double S { get; set; } //Range 0..1
        public double V { get; set; } //Range 0..1

        /// <summary>
        /// Creates a new color with HSV-Parameters
        /// </summary>
        /// <param name="_a">Alpha</param>
        /// <param name="_h">Hue</param>
        /// <param name="_s">Saturation</param>
        /// <param name="_v">Value</param>
        public LedColorHSV(double _h, double _s, double _v)
        {
            H = _h;
            S = _s;
            V = _v;
        }

        public override LedColorRGB GetColorRGB()
        {
            int hi = Convert.ToInt32(Math.Floor(H / 60)) % 6;
            double f = H / 60 - Math.Floor(H / 60);

            double value = V * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - S));
            byte q = Convert.ToByte(value * (1 - f * S));
            byte t = Convert.ToByte(value * (1 - (1 - f) * S));

            if (hi == 0)
                return new LedColorRGB(v, t, p);
            else if (hi == 1)
                return new LedColorRGB(q, v, p);
            else if (hi == 2)
                return new LedColorRGB(p, v, t);
            else if (hi == 3)
                return new LedColorRGB(p, q, v);
            else if (hi == 4)
                return new LedColorRGB(t, p, v);
            else
                return new LedColorRGB(v, p, q);
        }

        public override LedColorHSV GetColorHSV() { return this; }
    }

}