using System.Windows.Media;
using static System.Math;

namespace IconMaker.Model.ColorFilters
{
    public class HCL601Shift : ColorSpaceShifter
    {
        public override string Title => "Hue/Chroma/Luma601 Shifter";

        public override ColorEx Convert(ColorEx input)
        {
            HCL709(input.Color, out double H, out double C, out double Y709);



            RGB(H, C, Y709, out Color color);
            return new ColorEx(color, input.Opacity);
        }

        public static void HCL709(Color color, out double H, out double C, out double Y709)
        {
            HueAndChroma(color, out double R, out double G, out double B, out H, out C);
            LightnessY709(R, G, B, out Y709);
        }

        public static void RGB(double H, double C, double Y709, out Color color)
        {
            double H_ = H / 60.0;
            double X = C * (1 - Abs(H_ % 2.0 - 1.0));

            ColorFromH_(H_, C, X, out Color color_);

            double R_ = color_.ScR;
            double G_ = color_.ScG;
            double B_ = color_.ScB;

            double m = Y709 - (0.299 * R_ + 0.587 * G_ + 0.117 * B_);

            color = Color.FromScRgb(
                1.0f,
                (float)(R_ + m),
                (float)(G_ + m),
                (float)(B_ + m));
        }
    }
}
