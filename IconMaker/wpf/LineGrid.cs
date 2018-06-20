using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IconMaker.wpf
{
    public class LineGrid : ContentControl
    {
        public static readonly DependencyProperty GridCountProperty = DependencyProperty.Register(
            nameof(GridCount), typeof(uint), typeof(LineGrid),
            new FrameworkPropertyMetadata(16u, FrameworkPropertyMetadataOptions.AffectsRender));

        public uint GridCount
        {
            get => (uint)GetValue(GridCountProperty);
            set => SetValue(GridCountProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double dx = ActualWidth / GridCount;
            double dy = ActualHeight / GridCount;
            Pen p = new Pen(Brushes.DarkGray, 1.0);

            for (int n = 0; n < GridCount + 1; ++n)
            {
                dc.DrawLine(p, new Point(0, n * dy), new Point(ActualWidth, n * dy));
                dc.DrawLine(p, new Point(n * dx, 0), new Point(n * dy, ActualHeight));
            }
        }
    }
}
