using System;
using System.Globalization;

namespace LedMusic2.LedColors
{
    public class LedColorHSV
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

        public string ToHsvString()
        {
            return string.Format("{0},{1},{2}", H.ToString(CultureInfo.InvariantCulture),
                S.ToString(CultureInfo.InvariantCulture), V.ToString(CultureInfo.InvariantCulture));
        }

        public LedColor GetColorRGB()
        {
            int hi = Convert.ToInt32(Math.Floor(H / 60)) % 6;
            double f = H / 60 - Math.Floor(H / 60);

            double value = V * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - S));
            byte q = Convert.ToByte(value * (1 - f * S));
            byte t = Convert.ToByte(value * (1 - (1 - f) * S));

            if (hi == 0)
                return new LedColor(v, t, p);
            else if (hi == 1)
                return new LedColor(q, v, p);
            else if (hi == 2)
                return new LedColor(p, v, t);
            else if (hi == 3)
                return new LedColor(p, q, v);
            else if (hi == 4)
                return new LedColor(t, p, v);
            else
                return new LedColor(v, p, q);
        }

        protected void SetRGB(byte r, byte g, byte b)
        {
            var c = new LedColor(r, g, b);
            var h = c.GetColorHSV();
            H = h.H;
            S = h.S;
            V = h.V;
        }

    }
}
