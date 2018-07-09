using System.Windows;
using System.Windows.Media.Effects;

namespace IconMaker.wpf
{
    public class HueRingEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new PixelShader();

        static HueRingEffect()
        {
#if DEBUG
            _pixelShader.UriSource = Global.MakePackUri("HueRing.cso");
#else
            _pixelShader.UriSource = Global.MakePackUri("HueRing.cso");
#endif
        }

        public HueRingEffect()
        {
            PixelShader = _pixelShader;
            UpdateShaderValue(SaturationProperty);
            UpdateShaderValue(ValueProperty);
        }

        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
            nameof(Saturation), typeof(double), typeof(HueRingEffect),
            new UIPropertyMetadata(1.0d, PixelShaderConstantCallback(0)));

        private static void SaturationChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PixelShaderConstantCallback(0)(d, e);
        }

        public double Saturation
        {
            get => (double) GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(HueRingEffect),
            new UIPropertyMetadata(1.0d, PixelShaderConstantCallback(1)));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
