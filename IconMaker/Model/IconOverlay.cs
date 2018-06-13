using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using IconMaker.Annotations;

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
