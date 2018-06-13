using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using IconMaker.Annotations;

namespace IconMaker.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        public void AddOverlay(string relativeFileName, Viewbox viewbox)
        {
            string[] parts = relativeFileName.Split(@"\".ToCharArray(), 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return;

            string libraryName = parts[0];
            IconLibrary library = FindLibrary(libraryName);

            string iconName = new string(parts[2].TakeWhile(c => c != '.').ToArray());
            parts = iconName.Split("_".ToCharArray());
            if (parts.Length != 2)
                return;

            string overlayName = parts[0];
            IconOverlay overlay = library.FindOverlay(overlayName);

            switch (parts[1])
            {
                case "TL":
                    overlay.TL = viewbox;
                    break;
                case "TR":
                    overlay.TR = viewbox;
                    break;
                case "BL":
                    overlay.BL = viewbox;
                    break;
                case "BR":
                    overlay.BR = viewbox;
                    break;
            }
        }

        public void AddIcon(string relativeFileName, Viewbox viewbox)
        {
            string[] parts = relativeFileName.Split(@"\".ToCharArray(), 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return;

            string libraryName = parts[0];
            IconLibrary library = FindLibrary(libraryName);

            string categoryName = parts[1];
            Category category = library.FindCategory(categoryName);

            string iconName = new string(parts[1].TakeWhile(c => c != '.').ToArray());
            category.FindIcon(iconName, viewbox);
        }

        public ObservableCollection<IconLibrary> Libraries { get; } = new ObservableCollection<IconLibrary>();

        private IconLibrary FindLibrary(string libraryName)
        {
            IconLibrary library = Libraries.FirstOrDefault(l => l.Name.Equals(libraryName));
            if (library != null)
                return library;

            library = new IconLibrary(libraryName);
            Libraries.InsertSorted(library);
            return library;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
