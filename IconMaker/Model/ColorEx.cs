using System;
using System.Windows.Media;

namespace IconMaker.Model
{
    public struct ColorEx : IFormattable, IEquatable<ColorEx>
    {
        public void Deconstruct(out Color color, out double opacity)
        {
            color = Color;
            opacity = Opacity;
        }

        public ColorEx(Color color, double opacity)
        {
            Color = color;
            Opacity = opacity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ColorEx ex && Equals(ex);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Color.GetHashCode() * 397) ^ Opacity.GetHashCode();
            }
        }

        public static bool operator ==(ColorEx left, ColorEx right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ColorEx left, ColorEx right)
        {
            return !left.Equals(right);
        }

        public Color Color { get; }
        public double Opacity { get; }


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
}
