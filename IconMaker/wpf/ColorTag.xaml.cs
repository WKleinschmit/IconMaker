using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IconMaker.Model;

namespace IconMaker.wpf
{
    public partial class ColorTag
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(ColorEx), typeof(ColorTag),
            new PropertyMetadata(default(ColorEx), OnColorChanged));

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorTag colorTag && e.NewValue is ColorEx colorEx)
            {
                colorTag.Rectangle.Fill = new SolidColorBrush(colorEx.Color);
                colorTag.TextBlock.Text = colorEx.ToString();
            }
        }

        public ColorEx Color
        {
            get => (ColorEx) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public ColorTag()
        {
            InitializeComponent();
        }
    }
}
