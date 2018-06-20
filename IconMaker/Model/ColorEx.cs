using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace IconMaker.Model
{
    public struct ColorEx : IFormattable, IEquatable<ColorEx>
    {
        public Color Color { get; set; }
        public double Opacity { get; set; }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{Color.ToString(formatProvider)} ({Opacity.ToString("P0", formatProvider)})";
        }

        public bool Equals(ColorEx other)
        {
            return Color.Equals(other.Color) && Opacity.Equals(other.Opacity);
        }

        public override string ToString()
        {
            return $"{Color} ({Opacity:P0})";
        }
    }

    public class ColorMap : Dictionary<ColorEx, ColorEx>
    {

    }
}
