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

namespace IconMaker.wpf
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:IconMaker.wpf"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:IconMaker.wpf;assembly=IconMaker.wpf"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:AspectRatioContentControl/>
    ///
    /// </summary>
    public class AspectRatioContentControl : ContentControl
    {
        static AspectRatioContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AspectRatioContentControl), new FrameworkPropertyMetadata(typeof(AspectRatioContentControl)));
        }

        private ContentPresenter _contentPresenter;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName("PART_Content", this) is ContentPresenter contentPresenter)
                _contentPresenter = contentPresenter;

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == PaddingProperty && _contentPresenter != null)
                UpdateLayout(RenderSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size sz = base.MeasureOverride(constraint);
            return sz;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Size ds = DesiredSize;

            UpdateLayout(sizeInfo.NewSize);
        }

        private void UpdateLayout(Size newSize)
        {
            double spaceX = newSize.Width - Padding.Left - Padding.Right;
            if (spaceX < 0)
                spaceX = 0;

            double spaceY = newSize.Height - Padding.Top - Padding.Bottom;
            if (spaceY < 0)
                spaceY = 0;

            if (spaceY > spaceX)
            {
                _contentPresenter.Width = _contentPresenter.Height = spaceX;
                _contentPresenter.SetValue(Canvas.LeftProperty, Padding.Left);
                _contentPresenter.SetValue(Canvas.TopProperty, Padding.Top + (spaceY - spaceX) / 2.0);
            }
            else
            {
                _contentPresenter.Width = _contentPresenter.Height = spaceY;
                _contentPresenter.SetValue(Canvas.LeftProperty, Padding.Left + (spaceX - spaceY) / 2.0);
                _contentPresenter.SetValue(Canvas.TopProperty, Padding.Top);
            }
        }
    }
}
