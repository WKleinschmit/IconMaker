using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IconMaker.Annotations;
using static System.Math;

namespace IconMaker.wpf
{
    public class ColorWheel : Control, INotifyPropertyChanged
    {
        private EllipseGeometry _geo1;
        private EllipseGeometry _geo2;
        private HueRingEffect _effect;

        static ColorWheel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ColorWheel),
                new FrameworkPropertyMetadata(typeof(ColorWheel)));
        }

        public override void OnApplyTemplate()
        {
            _geo1 = (EllipseGeometry)Template.FindName("Geo1", this);
            _geo2 = (EllipseGeometry)Template.FindName("Geo2", this);
            _effect = (HueRingEffect)Template.FindName("HueRingEffect", this);

            OuterRadiusPropertyChanged(this, new DependencyPropertyChangedEventArgs(OuterRadiusProperty, null, OuterRadius));
            InnerRadiusPropertyChanged(this, new DependencyPropertyChangedEventArgs(InnerRadiusProperty, null, InnerRadius));
            SaturationPropertyChanged(this, new DependencyPropertyChangedEventArgs(SaturationProperty, null, Saturation));
            ValuePropertyChanged(this, new DependencyPropertyChangedEventArgs(ValueProperty, null, Value));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Recalculate();
        }

        private void Recalculate()
        {
            _geo1.RadiusX = _geo1.RadiusY = Min(ActualWidth, ActualHeight) / 2.0 * OuterRadius;
            _geo2.RadiusX = _geo2.RadiusY = Min(ActualWidth, ActualHeight) / 2.0 * InnerRadius;
            _geo1.Center = _geo2.Center = new Point(ActualWidth / 2.0, ActualHeight / 2.0);
        }

        public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register(
            nameof(OuterRadius), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(1.0, OuterRadiusPropertyChanged), ValidateOuterRadius);

        private static void OuterRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorWheel This && This._geo1 != null && This._geo2 != null)
            {
                This.Recalculate();
            }
        }

        private static bool ValidateOuterRadius(object value)
        {
            if (!(value is double doubleValue))
                return false;

            return doubleValue >= 0.0 && doubleValue <= 1.0;
        }

        public double OuterRadius
        {
            get => (double)GetValue(OuterRadiusProperty);
            set => SetValue(OuterRadiusProperty, value);
        }

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            nameof(InnerRadius), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(0.75, InnerRadiusPropertyChanged), ValidateInnerRadius);

        private static void InnerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorWheel This && This._geo1 != null && This._geo2 != null)
            {
                This.Recalculate();
            }
        }

        private static bool ValidateInnerRadius(object value)
        {
            if (!(value is double doubleValue))
                return false;

            return doubleValue >= 0.0 && doubleValue <= 1.0;
        }

        public double InnerRadius
        {
            get => (double)GetValue(InnerRadiusProperty);
            set => SetValue(InnerRadiusProperty, value);
        }

        public void OnInnerRadiusChanged()
        {
            Recalculate();
            InvalidateVisual();
        }

        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
            nameof(Saturation), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(1.0, SaturationPropertyChanged), ValidateColorValue);

        private static void SaturationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorWheel This && This._effect != null)
            {
                This._effect.Saturation = (double) e.NewValue;
            }
        }

        public double Saturation
        {
            get => (double) GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(1.0, ValuePropertyChanged), ValidateColorValue);

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorWheel This && This._effect != null)
            {
                This._effect.Value = (double)e.NewValue;
            }
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static bool ValidateColorValue(object value)
        {
            if (!(value is double doubleValue))
                return false;

            return doubleValue >= 0.0 && doubleValue <= 1.0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
