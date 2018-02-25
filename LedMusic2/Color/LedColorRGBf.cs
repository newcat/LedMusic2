﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2.Color
{

    public class LedColorRGBf : LedColor
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

        private float GetValue()
        {
            return Math.Max(Math.Max(R, G), B);
        }

    }

}