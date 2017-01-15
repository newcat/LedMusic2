using LedMusic2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.Helpers
{
    class TypeConverter
    {

        private static List<SingleTypeConverter> converters = new List<SingleTypeConverter>();

        public static bool CanConvert(Type input, Type output)
        {
            return input == output || converters.Exists((x) => x.InputType == input && x.OutputType == output);
        }

        public static object Convert(Type input, Type output, object value)
        {
            if (input == output)
                return value;

            var converter = converters.FirstOrDefault((x) => x.InputType == input && x.OutputType == output);
            if (converter != null)
                return converter.ConversionFunc(value);
            else
                return null;
        }

        public static void Initialize()
        {


            //Booleans
            converters.Add(new SingleTypeConverter(typeof(double), typeof(bool),
                (x) => ((double)x) > 0));

            //Colors
            converters.Add(new SingleTypeConverter(typeof(double), typeof(Color),
                (x) =>
                {
                    byte v = (byte)Math.Max(0, Math.Min(255, (double)x * 255));
                    return new ColorRGB(v, v, v);
                }));

            //Color Array
            converters.Add(new SingleTypeConverter(typeof(Color), typeof(Color[]),
                (x) =>
                {
                    int ledCount = GlobalProperties.Instance.LedCount;
                    Color[] arr = new Color[ledCount];
                    for (int i = 0; i < ledCount; i++)
                        arr[i] = (Color)x;
                    return arr;
                }));

        }

        private class SingleTypeConverter
        {

            public Type InputType { get; private set; }
            public Type OutputType { get; private set; }
            public Func<object, object> ConversionFunc { get; private set; }

            public SingleTypeConverter(Type input, Type output, Func<object, object> f)
            {
                InputType = input;
                OutputType = output;
                ConversionFunc = f;
            }

        }

    }
}
