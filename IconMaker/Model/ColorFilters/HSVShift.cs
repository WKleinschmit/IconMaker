using System.Windows.Media;
using static System.Math;

namespace IconMaker.Model.ColorFilters
{
    public class HSVShift : ColorSpaceShifter
    {
        public override string Title => "Hue/Saturation/Value Shifter";

        public override ColorEx Convert(ColorEx input)
        {
            HSV(input.Color, out double H, out double S, out double V);



            RGB(H, S, V, out Color color);
            return new ColorEx(color, input.Opacity);
        }

        public static void HSV(Color color, out double H, out double S, out double V)
        {
            HueAndChroma(color, out double R, out double G, out double B, out H, out double C);
            LightnessV(R, G, B, out V);
            SaturationHSV(C, V, out S);
        }

        public static void RGB(double H, double S, double V, out Color color)
        {
            double C = V * S;
            double H_ = H / 60.0;
            double X = C * (1 - Abs(H_ % 2.0 - 1.0));

            ColorFromH_(H_, C, X, out color);
        }
    }
}
