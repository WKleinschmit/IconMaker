using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;
using IconMaker.Annotations;

namespace IconMaker.Model
{
    public class Icon : IComparable<Icon>, INotifyPropertyChanged
    {
        internal Icon(string title, Viewbox viewbox)
        {
            Title = title;
            Viewbox = viewbox;
        }

        internal Icon(XElement eltIcon)
        {
            Title = eltIcon.Attribute("title")?.Value;
            LoadIcon(eltIcon);
        }

        private async void LoadIcon(XContainer eltIcon)
        {
            Viewbox = await App.DispatchFunc(() => (Viewbox)XamlReader.Load(eltIcon.FirstNode.CreateReader()));
        }

        public string Title { get; }
        public Viewbox Viewbox { get; private set; }

        public int CompareTo(Icon other)
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
