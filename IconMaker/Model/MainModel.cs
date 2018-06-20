using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using IconMaker.Annotations;
using IconMaker.wpf;
using PropertyChanged;
using static System.Math;

namespace IconMaker.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        public static readonly XNamespace P = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        public static readonly XNamespace X = XNamespace.Get("http://schemas.microsoft.com/winfx/2006/xaml");

        public MainModel()
        {
            CmdNew = new RelayCommand(OnNew);
            CmdOpen = new RelayCommand(OnOpen);
            CmdSave = new RelayCommand(OnSave, CanSave);
            CmdPrevIcon = new RelayCommand(OnPrevIcon, CanPrevIcon);
            CmdNextIcon = new RelayCommand(OnNextIcon, CanNextIcon);
            CmdAddToCollection = new RelayCommand(OnAddToCollection, CanAddToCollection);
            CmdCloseCollection = new RelayCommand(OnCloseCollection);
        }

        private void OnCloseCollection(object obj)
        {
            if (obj is Collection collection)
                Collections.Remove(collection);
        }

        private void OnAddToCollection(object obj)
        {
            for (int n = 0; n < SelectedCount; n++)
            {
                ColorMap colorMap = new ColorMap();
                Viewbox viewbox = CreateViewbox(n, out string title, colorMap);
                CurrentCollection.Icons.Add(new CollectionIcon(viewbox) { Title = title });
            }
        }

        private bool CanAddToCollection(object arg)
        {
            return CurrentCollection != null && SelectedCount > 0;
        }

        private void OnPrevIcon(object obj)
        {
            SelectedIndex = Max(0, SelectedIndex - 1);
        }

        private bool CanPrevIcon(object arg)
        {
            return SelectedIndex > 0;
        }

        private void OnNextIcon(object obj)
        {
            SelectedIndex = Min(SelectedCount - 1, SelectedIndex + 1);
        }

        private bool CanNextIcon(object arg)
        {
            return SelectedIndex < SelectedCount - 1;
        }

        private void OnNew(object obj)
        {
            Collections.Add(new Collection());
        }

        private void OnOpen(object obj)
        {

        }

        private bool CanSave(object arg)
        {
            return CurrentCollection != null;
        }

        private void OnSave(object obj)
        {

        }

        public RelayCommand CmdNew { get; }
        public RelayCommand CmdOpen { get; }
        public RelayCommand CmdSave { get; }
        public RelayCommand CmdPrevIcon { get; }
        public RelayCommand CmdNextIcon { get; }
        public RelayCommand CmdAddToCollection { get; }
        public RelayCommand CmdCloseCollection { get; }

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

        public ColorMap CurrentColorMap { get; set; }

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

            CurrentColorMap = new ColorMap();
            Viewbox viewbox = CreateViewbox(SelectedIndex, out string _, CurrentColorMap);
            if (viewbox != null)
                Preview.Children.Add(viewbox);
        }

        private Viewbox CreateViewbox(int index, out string title, ColorMap colorMap)
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
                    ApplyColorMap(overlayXaml, colorMap);

                    viewbox = (Viewbox)XamlReader.Load(overlayXaml.CreateReader());
                    title = $"{SelectedIcons[iconIndex].Title}_{SelectedOverlays[overlayIndex].Title}";
                }
                else
                {
                    viewbox = null;
                    title = null;
                }
            }
            else
            {
                Viewbox icon = SelectedIcons[index].Viewbox;
                XDocument iconXaml = XDocument.Parse(XamlWriter.Save(icon));
                ApplyColorMap(iconXaml, colorMap);

                viewbox = (Viewbox)XamlReader.Load(iconXaml.CreateReader());


                title = $"{SelectedIcons[index].Title}";
            }

            return viewbox;
        }

        private static readonly ColorConverter colorConverter = new ColorConverter();

        private static void ApplyColorMap(XNode xaml, ColorMap colorMap)
        {
            if (!(xaml.XPathEvaluate("//@*") is IEnumerable attributes))
                return;

            IEnumerable<XAttribute> colorValues = attributes.OfType<XAttribute>().Where(a => a.Value.StartsWith("#"));

            foreach (XAttribute colorAttribute in colorValues)
            {
                if (!(colorConverter.ConvertFromInvariantString(colorAttribute.Value) is Color color))
                    continue;

                if (!(colorAttribute.Parent is XElement parent))
                    continue;

                double opacity = 1.0;
                if (parent.Attribute("Opacity")?.Value is string opacityString)
                    opacity = double.Parse(opacityString, CultureInfo.InvariantCulture);

                ColorEx colorEx0 = new ColorEx { Color = color, Opacity = opacity };

                if (colorMap.TryGetValue(colorEx0, out ColorEx colorEx1) && colorConverter.ConvertToInvariantString(colorEx1.Color) is string newValue)
                {
                    colorAttribute.Value = newValue;
                    parent.SetAttributeValue("Opacity", colorEx1.Opacity);
                }
                else
                    colorMap[colorEx0] = colorEx0;
            }
        }

        public Grid Preview { get; set; } = new Grid() { Background = Brushes.Transparent };

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

        public ObservableCollection<Collection> Collections { get; } = new ObservableCollection<Collection>();
        public Collection CurrentCollection { get; set; }
    }
}
