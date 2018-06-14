﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using IconMaker.Annotations;

namespace IconMaker.Model
{
    public class Category : IComparable<Category>, INotifyPropertyChanged
    {
        internal Category(string name, IconLibrary library)
        {
            Name = name;
            Library = library;
        }

        public string Name { get; }
        public IconLibrary Library { get; }
        public ObservableCollection<Icon> Icons { get; } = new ObservableCollection<Icon>();

        public int CompareTo(Category other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public Icon FindIcon(string iconName, Viewbox viewbox)
        {
            Icon icon = Icons.FirstOrDefault(i => i.Title.Equals(iconName));
            if (icon != null)
                return icon;

            icon = new Icon(iconName, viewbox);
            Icons.InsertSorted(icon);
            return icon;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}