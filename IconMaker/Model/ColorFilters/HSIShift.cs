using System.Windows.Media;
using static System.Math;

namespace IconMaker.Model.ColorFilters
{
    public class HSIShift : ColorSpaceShifter
    {
        public override string Title => "Hue/Saturation/Intensity Shifter";
        public override ColorEx Convert(ColorEx input)
        {
            HSI(input.Color, out double H, out double S, out double I);



            RGB(H, S, I, out Color color);
            return new ColorEx(color, input.Opacity);
        }

        public static void HSI(Color color, out double H, out double S, out double I)
        {
            HueAndChroma(color, out double R, out double G, out double B, out double _, out double m, out H, out double C);
            LightnessI(R, G, B, out I);
            SaturationHSI(C, I, m, out S);
        }

        public static void RGB(double H, double S, double I, out Color color)
        {
            double H_ = H / 60;
            double Z = 1 - Abs(H_ % 2 - 1);
            double C = (3.0 * I * S) / (1 + Z);
            double X = C * Z;

            ColorFromH_(H_, C, X, out color);
        }
    }
}
