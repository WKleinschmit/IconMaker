using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using IconMaker.Annotations;

namespace IconMaker.Model
{
    public class IconLibrary : IComparable<IconLibrary>, INotifyPropertyChanged
    {
        internal IconLibrary(string name)
        {
            Name = name;
        }

        internal IconLibrary(XElement eltLibrary)
        {
            Name = eltLibrary.Attribute("name")?.Value;

            foreach (XElement eltOverlay in eltLibrary.Elements(MainModel.NSIconMaker + "Overlay"))
                Overlays.Add(new IconOverlay(eltOverlay));

            foreach (XElement eltCategory in eltLibrary.Elements(MainModel.NSIconMaker + "Category"))
                Categories.Add(new Category(eltCategory, this));
        }

        public string Name { get; }
        public ObservableCollection<IconOverlay> Overlays { get; } = new ObservableCollection<IconOverlay>();
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public int CompareTo(IconLibrary other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public IconOverlay FindOverlay(string overlayName)
        {
            IconOverlay overlay = Overlays.FirstOrDefault(o => o.Title.Equals(overlayName));
            if (overlay != null)
                return overlay;

            overlay = new IconOverlay(overlayName);
            Overlays.InsertSorted(overlay);
            return overlay;
        }

        public Category FindCategory(string categoryName)
        {
            Category category = Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category != null)
                return category;

            category = new Category(categoryName, this);
            Categories.InsertSorted(category);

            return category;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
