using LedMusic2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.Helpers
{
    class TypeConverter
    {

        private static List<SingleTypeConverter> converters = new List<SingleTypeConverter>();
        private static List<ConverterCombination> combinations = new List<ConverterCombination>();

        public static bool CanConvert(Type input, Type output)
        {
            return input == output ||
                converters.Exists((x) => x.InputType == input && x.OutputType == output) ||
                combinations.Exists((x) => x.InputType == input && x.OutputType == output);
        }

        public static object Convert(Type input, Type output, object value)
        {
            if (input == output)
                return value;

            var converter = converters.FirstOrDefault((x) => x.InputType == input && x.OutputType == output);
            if (converter != null)
                return converter.ConversionFunc(value);
            else
            {
                var comb = combinations.FirstOrDefault((x) => x.InputType == input && x.OutputType == output);
                if (comb != null)
                {
                    foreach (var conv in comb.Converters)
                    {
                        value = conv.ConversionFunc(value);
                    }
                    return value;
                } else
                {
                    return null;
                }
            }
                
        }

        public static void Initialize()
        {


            //Booleans
            var doubleToBool = new SingleTypeConverter(typeof(double), typeof(bool),
                (x) => ((double)x) > 0);
            converters.Add(doubleToBool);

            //Doubles
            var boolToDouble = new SingleTypeConverter(typeof(bool), typeof(double),
                (x) => (bool)x ? 1d : 0d);
            converters.Add(boolToDouble);

            //Colors
            var doubleToColor = new SingleTypeConverter(typeof(double), typeof(LedColor),
                (x) =>
                {
                    byte v = (byte)Math.Max(0, Math.Min(255, (double)x * 255));
                    return new LedColorRGB(v, v, v);
                });
            converters.Add(doubleToColor);

            var colorArrayToColor = new SingleTypeConverter(typeof(LedColor[]), typeof(LedColor),
                (x) =>
                {

                    if (x == null)
                        return new LedColorRGB(0, 0, 0);

                    var arr = (LedColor[])x;
                    if (arr.Length > 0)
                        return arr[0];
                    else
                        return new LedColorRGB(0, 0, 0);
                });
            converters.Add(colorArrayToColor);

            //Color Array
            var colorToColorArray = new SingleTypeConverter(typeof(LedColor), typeof(LedColor[]),
                (x) => new LedColor[] { (LedColor)x });
            converters.Add(colorToColorArray);

            var doubleToColorArray = new ConverterCombination(typeof(double), typeof(LedColor[]));
            doubleToColorArray.Converters.Add(doubleToColor);
            doubleToColorArray.Converters.Add(colorToColorArray);
            combinations.Add(doubleToColorArray);

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

        private class ConverterCombination
        {

            private List<SingleTypeConverter> _converters = new List<SingleTypeConverter>();
            public List<SingleTypeConverter> Converters { get { return _converters; } }

            public Type InputType { get; private set; }
            public Type OutputType { get; private set; }

            public ConverterCombination(Type input, Type output)
            {
                InputType = input;
                OutputType = output;
            }

        }

    }
}
