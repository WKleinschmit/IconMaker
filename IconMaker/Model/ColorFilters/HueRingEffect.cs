using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace IconMaker.Model.ColorFilters
{
    public class HueRingEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new PixelShader();

        static HueRingEffect()
        {
#if DEBUG
            _pixelShader.UriSource = Global.MakePackUri("bin/debug/hue.cso");
#else
            _pixelShader.UriSource = Global.MakePackUri("bin/release/hue.cso");
#endif
        }

        public HueRingEffect()
        {
            PixelShader = _pixelShader;
        }
        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(HueRingEffect), 0);
    }
}
