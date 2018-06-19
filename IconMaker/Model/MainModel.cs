using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using IconMaker.Annotations;
using PropertyChanged;

namespace IconMaker.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        public static readonly XNamespace P = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        public static readonly XNamespace X = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml");

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

            string iconName = new string(parts[2].TakeWhile(c => c != '.').ToArray());
            category.FindIcon(iconName, viewbox);
        }

        public ObservableCollection<IconLibrary> Libraries { get; } = new ObservableCollection<IconLibrary>();

        [AlsoNotifyFor(nameof(HasCategory))]
        public Category Category { get; set; }
        public bool HasCategory => Category != null;

        [AlsoNotifyFor(nameof(HasLibrary))]
        public IconLibrary Library { get; set; }
        public bool HasLibrary => Library != null;

        public Icon[] SelectedIcons { get; set; }

        public void OnSelectedIconsChanged()
        {
            SelectionChanged();
        }

        public IconOverlay[] SelectedOverlays { get; set; }

        public void OnSelectedOverlaysChanged()
        {
            SelectionChanged();
        }

        public void SelectionChanged()
        {
            Icon[] selectedIcons = SelectedIcons ?? new Icon[0];
            int iconCount = selectedIcons.Length;

            IconOverlay[] selectedOverlays = SelectedOverlays ?? new IconOverlay[0];
            OverlayCount = selectedOverlays.Length;

            HasIconSelection = selectedIcons.Length > 0;
            HasOverlaySelection = OverlayCount > 0;
            SelectedCount = iconCount;

            SuspendPreview();

            if (HasOverlaySelection)
            {
                SelectedCount *= OverlayCount;
                if (OverlayPosition == OverlayPosition.None)
                    OverlayPosition = OverlayPosition.BR;
            }
            else
                OverlayPosition = OverlayPosition.None;
            SelectedIndex = 0;

            ResumePreview();
        }

        [DoNotCheckEquality]
        public int OverlayCount { get; set; }

        [DoNotCheckEquality]
        public bool HasIconSelection { get; set; }

        [DoNotCheckEquality]
        public bool HasOverlaySelection { get; set; }

        [DoNotCheckEquality]
        public int SelectedCount { get; set; }

        [DoNotCheckEquality]
        public int SelectedIndex { get; set; }

        public void OnSelectedIndexChanged()
        {
            UpdatePreview();
        }

        private int _suspendCount;
        private bool _needsUpdate;
        public void SuspendPreview()
        {
            ++_suspendCount;
        }

        public void ResumePreview()
        {
            if (--_suspendCount == 0 && _needsUpdate)
                UpdatePreview();
        }


        private void UpdatePreview()
        {
            if (_suspendCount > 0)
            {
                _needsUpdate = true;
                return;
            }

            _needsUpdate = false;

            Preview.Children.Clear();
            if (!HasIconSelection)
                return;

            Viewbox viewbox = CreateViewbox(SelectedIndex);
            if (viewbox != null)
                Preview.Children.Add(viewbox);
        }

        private Viewbox CreateViewbox(int index)
        {
            Viewbox viewbox;
            if (HasOverlaySelection)
            {
                int overlayIndex = index % OverlayCount;
                Viewbox overlay = SelectedOverlays[overlayIndex][OverlayPosition];

                int iconIndex = index / OverlayCount;
                Viewbox icon = SelectedIcons[iconIndex].Viewbox;

                XDocument overlayXaml = XDocument.Parse(XamlWriter.Save(overlay));
                XElement eltIconInOverlay = overlayXaml.Descendants(P + "Canvas").FirstOrDefault(e => e.Attribute(X + "Uid")?.Value == "Icon");

                XDocument iconXaml = XDocument.Parse(XamlWriter.Save(icon));
                XElement eltIconInIcon = iconXaml.Descendants(P + "Canvas").FirstOrDefault(e => e.Attribute(X + "Uid")?.Value == "Icon");

                if (eltIconInOverlay != null && eltIconInIcon != null)
                {
                    foreach (XElement element in eltIconInIcon.Elements())
                    {
                        eltIconInOverlay.Add(element);
                    }

                    viewbox = (Viewbox)XamlReader.Load(overlayXaml.CreateReader());
                }
                else
                    viewbox = null;
            }
            else
            {
                Icon icon = SelectedIcons[SelectedIndex];

                string s = XamlWriter.Save(icon.Viewbox);
                StringReader stringReader = new StringReader(s);
                XmlReader xmlReader = XmlReader.Create(stringReader);

                viewbox = (Viewbox)XamlReader.Load(xmlReader);
            }

            return viewbox;
        }

        public Grid Preview { get; set; } = new Grid() { Background = Brushes.White };

        [DoNotCheckEquality]
        public OverlayPosition OverlayPosition { get; set; }

        public void OnOverlayPositionChanged()
        {
            UpdatePreview();
        }

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
