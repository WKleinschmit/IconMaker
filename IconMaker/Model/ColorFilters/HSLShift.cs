using System.Windows.Media;
using static System.Math;

namespace IconMaker.Model.ColorFilters
{
    public class HSLShift : ColorSpaceShifter
    {
        public override string Title => "Hue/Saturation/Lightness Shifter";

        public override ColorEx Convert(ColorEx input)
        {
            HSL(input.Color, out double H, out double S, out double L);



            RGB(H, S, L, out Color color);
            return new ColorEx(color, input.Opacity);
        }

        public static void HSL(Color color, out double H, out double S, out double L)
        {
            HueAndChroma(color, out double R, out double G, out double B, out H, out double C);
            LightnessL(R, G, B, out L);
            SaturationHSL(C, L, out S);
        }

        public static void RGB(double H, double S, double L, out Color color)
        {
            double C = (1 - Abs(2 * L - 1)) * S;
            double H_ = H / 60;
            double X = C * (1 - Abs(H_ % 2.0 - 1.0));

            ColorFromH_(H_, C, X, out color);
        }
    }
}
