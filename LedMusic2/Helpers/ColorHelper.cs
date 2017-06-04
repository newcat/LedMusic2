using LedMusic2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2.Helpers
{
    static class ColorHelper
    {

        public static LedColorRGB Mix(LedColor aa, LedColor bb, double factor)
        {

            var a = aa.GetColorRGB();
            var b = bb.GetColorRGB();

            double factorm = 1.0 - factor;
            int red = (int)(factorm * a.R + factor * b.R);
            int green = (int)(factorm * a.G + factor * b.G);
            int blue = (int)(factorm * a.B + factor * b.B);
            return clamp(red, green, blue);

        }

        public static LedColorRGB Add(LedColor aa, LedColor bb, double factor)
        {

            var a = aa.GetColorRGB();
            var b = bb.GetColorRGB();

            int red = (int)(a.R + factor * b.R);
            int green = (int)(a.G + factor * b.G);
            int blue = (int)(a.B + factor * b.B);
            return clamp(red, green, blue);

        }

        private static LedColorRGB clamp(int r, int g, int b)
        {
            return new LedColorRGB(
                (byte)Math.Max(0, Math.Min(255, r)),
                (byte)Math.Max(0, Math.Min(255, g)),
                (byte)Math.Max(0, Math.Min(255, b)));
        }

    }
}
