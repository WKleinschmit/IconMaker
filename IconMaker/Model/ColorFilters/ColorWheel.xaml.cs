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
using IconMaker.Annotations;
using PropertyChanged;
using static System.Math;

namespace IconMaker.Model.ColorFilters
{
    /// <summary>
    /// Interaction logic for ColorWheel.xaml
    /// </summary>
    public partial class ColorWheel : INotifyPropertyChanged
    {
        public ColorWheel()
        {
            InitializeComponent();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            OnPropertyChanged(nameof(OuterRadius));
            OnPropertyChanged(nameof(InnerRadius));
            OnPropertyChanged(nameof(CenterPoint));
        }

        public static readonly DependencyProperty OuterProperty = DependencyProperty.Register(
            nameof(Outer), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(1.0));

        [AlsoNotifyFor(nameof(OuterRadius))]
        public double Outer
        {
            get => (double)GetValue(OuterProperty);
            set => SetValue(OuterProperty, value);
        }

        public static readonly DependencyProperty InnerProperty = DependencyProperty.Register(
            nameof(Inner), typeof(double), typeof(ColorWheel),
            new PropertyMetadata(0.6));

        [AlsoNotifyFor(nameof(InnerRadius))]
        public double Inner
        {
            get => (double)GetValue(InnerProperty);
            set => SetValue(InnerProperty, value);
        }

        public double OuterRadius => Min(ActualWidth, ActualHeight) / 2 * Outer;
        public double InnerRadius => Min(ActualWidth, ActualHeight) / 2 * Inner;

        public Point CenterPoint => new Point(ActualWidth / 2, ActualHeight / 2);


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
