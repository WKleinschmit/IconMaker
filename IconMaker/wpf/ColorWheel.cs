using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;
using IconMaker.Annotations;
using static System.Math;

namespace IconMaker.wpf
{
    public class ColorWheel : Control, INotifyPropertyChanged
    {
        private EllipseGeometry _geo1;
        private EllipseGeometry _geo2;

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
            new PropertyMetadata(1.0), ValidateOuterRadius);

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

        public void OnOuterRadiusChanged()
        {
            Recalculate();
        }

        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            nameof(InnerRadius), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(0.75), ValidateInnerRadius);

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
