using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using IconMaker.Annotations;
using static System.Math;

namespace IconMaker.Model
{
    public class IconOverlay : IComparable<IconOverlay>, INotifyPropertyChanged
    {
        internal IconOverlay(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public Viewbox TL { get; internal set; }
        public Viewbox TR { get; internal set; }
        public Viewbox BL { get; internal set; }
        public Viewbox BR { get; internal set; }

        public Viewbox Viewbox { get; private set; }

        public void OnTLChanged()
        {
            if (Viewbox == null && TL != null)
                CreateViewbox(TL);
        }

        private async void CreateViewbox(Viewbox vb)
        {
            await App.DispatchAction(() =>
                {
                    if (vb.Child is Canvas cv)
                    {
                        string s = XamlWriter.Save(cv.Children[1]);
                        StringReader stringReader = new StringReader(s);
                        XmlReader xmlReader = XmlReader.Create(stringReader);
                        Canvas c = (Canvas)XamlReader.Load(xmlReader);

                        Size sz = Size.Empty;
                        foreach (UIElement cChild in c.Children.OfType<UIElement>())
                        {
                            cChild.Measure(new Size(2000, 2000));
                            sz = new Size(
                                Max(sz.Width, cChild.DesiredSize.Width),
                                Max(sz.Height, cChild.DesiredSize.Height));
                        }

                        c.Width = sz.Width;
                        c.Height = sz.Height;

                        Viewbox = new Viewbox
                        {
                            Stretch = Stretch.Uniform,
                            Child = c,
                        };
                    }
                }
            );
        }

        public int CompareTo(IconOverlay other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : string.Compare(Title, other.Title, StringComparison.Ordinal);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
