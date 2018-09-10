namespace LedMusic2.LedColors
{

    struct LedColorRGBf
    {

        public static LedColorRGBf FromLedColorRGB(LedColor c)
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

        public LedColor GetColorRGB()
        {
            byte red = (byte)(255 * R);
            byte green = (byte)(255 * G);
            byte blue = (byte)(255 * B);
            return new LedColor(red, green, blue);
        }

    }

}
