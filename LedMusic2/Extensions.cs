using LedMusic2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LedMusic2
{

    public static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable<T>
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        public static ColorRGB Overlay(this ColorRGB a, ColorRGB b)
        {
            double alphaA = a.getColorHSV().V;
            double alphaB = a.getColorHSV().V;
            byte red = Convert.ToByte(alphaA * a.R + (1 - alphaA) * b.R);
            byte green = Convert.ToByte(alphaA * a.G + (1 - alphaA) * b.G);
            byte blue = Convert.ToByte(alphaA * a.B + (1 - alphaA) * b.B);
            return new ColorRGB(red, green, blue);
        }

        public static ColorHSV Add(this ColorHSV a, ColorHSV b)
        {
            ColorRGB aRGB = a.getColorRGB();
            ColorRGB bRGB = b.getColorRGB();
            byte red = Convert.ToByte(Math.Min(aRGB.R + bRGB.R, 255));
            byte green = Convert.ToByte(Math.Min(aRGB.G + bRGB.G, 255));
            byte blue = Convert.ToByte(Math.Min(aRGB.B + bRGB.B, 255));
            return new ColorRGB(red, green, blue).getColorHSV();
        }

        public static T FindParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(obj);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent != null ? (T)parent : null;
        }

        public static List<T> FindDescendent<T>(this DependencyObject element) where T : DependencyObject
        {
            var f = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                if (child is T)
                    f.Add((T)child);

                f.AddRange(FindDescendent<T>(child));
            }
            return f;
        }

    }

}
