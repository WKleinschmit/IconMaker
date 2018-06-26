using System.Windows.Media;
using static System.Math;

namespace IconMaker.Model.ColorFilters
{
    public abstract class ColorSpaceShifter : IColorFilter
    {
        public abstract string Title { get; }
        public abstract ColorEx Convert(ColorEx input);

        private const double tol = 0.001;

        protected static void HueAndChroma(
            Color color,
            out double R, out double G, out double B,
            out double H, out double C)
        {
            HueAndChroma(color, out R, out G, out B, out _, out _, out H, out C);
        }

        protected static void HueAndChroma(
            Color color,
            out double R, out double G, out double B,
            out double M, out double m,
            out double H, out double C)
        {
            R = color.ScR;
            G = color.ScG;
            B = color.ScB;

            M = Max(R, Max(G, B));
            m = Min(R, Min(G, B));
            C = M - m;
            double H_ = 0.0;
            if (Abs(C) < tol)
            {
                if (Abs(M - R) < tol)
                    H_ = ((G - B) / C) % 6.0;
                if (Abs(M - G) < tol)
                    H_ = ((B - R) / C) + 2.0;
                if (Abs(M - B) < tol)
                    H_ = ((R - G) / C) + 4.0;
            }

            H = 60 * H_;
        }

        protected static void LightnessI(double R, double G, double B, out double I)
        {
            I = (R + G + B) / 3.0;
        }

        protected static void LightnessY709(double R, double G, double B, out double Y)
        {
            Y = 0.21 * R + 0.72 * G + 0.07 * B;
        }

        protected static void LightnessY601(double R, double G, double B, out double Y)
        {
            Y = 0.299 * R + 0.587 * G + 0.117 * B;
        }

        protected static void LightnessV(double R, double G, double B, out double V)
        {
            V = Max(R, Max(G, B));
        }

        protected static void LightnessL(double R, double G, double B, out double L)
        {
            double M = Max(R, Max(G, B));
            double m = Min(R, Min(G, B));
            L = (M + m) / 2.0;
        }

        protected static void SaturationHSV(double C, double V, out double Shsv)
        {
            Shsv = (Abs(V) < tol) ? 0.0 : C / V;
        }

        protected static void SaturationHSL(double C, double L, out double Shsl)
        {
            Shsl = (Abs(L - 1) < tol) ? 0.0 : C / Abs(2.0 * L - 1);
        }

        protected static void SaturationHSI(double C, double I, double m, out double Shsi)
        {
            Shsi = (Abs(I) < tol) ? 0.0 : 1.0 - (m / I);
        }

        protected static void ColorFromH_(double H_, double C, double X, out Color color)
        {
            if (H_ <= 1.0)
            {
                color = Color.FromScRgb(1.0f, (float)C, (float)X, 0.0f);
                return;
            }

            if (H_ <= 2.0)
            {
                color = Color.FromScRgb(1.0f, (float)X, (float)C, 0.0f);
                return;
            }

            if (H_ <= 3.0)
            {
                color = Color.FromScRgb(1.0f, 0.0f, (float)C, (float)X);
                return;
            }

            if (H_ <= 4.0)
            {
                color = Color.FromScRgb(1.0f, 0.0f, (float)X, (float)C);
                return;
            }

            if (H_ <= 5.0)
            {
                color = Color.FromScRgb(1.0f, (float)X, 0.0f, (float)C);
                return;
            }

            color = Color.FromScRgb(1.0f, (float)C, 0.0f, (float)X);
        }
    }
}
