using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using IconMaker.Model;

namespace IconMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainModel _model;

        public MainWindow()
        {
            InitializeComponent();

            if (!(DataContext is MainModel model))
                throw new ApplicationException();

            model.HasIconSelection = false;
            _model = model;
            _model.Owner = this;
        }


        private void LibraryTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Category category)
            {
                _model.Category = category;
                _model.Library = category.Library;
            }

            if (e.NewValue is IconLibrary library)
            {
                _model.Category = null;
                _model.Library = library;
            }
        }

        private void LbIcons_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ListBox lb) || e.Key < Key.A || e.Key > Key.Z)
                return;


            string searchLetter = e.Key.ToString();
            ObservableCollection<Icon> Things = (ObservableCollection<Icon>)lb.ItemsSource;
            ICollectionView cv = CollectionViewSource.GetDefaultView(Things);
            Icon thingToFind;
            if (lb.SelectedItem == null)
            {
                thingToFind = Things.FirstOrDefault(x => x.Title.StartsWith(searchLetter, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                int currentIndex = lb.Items.IndexOf((Icon)lb.SelectedItem);
                List<Icon> laterItems = Things.Skip(currentIndex + 1).ToList();
                thingToFind = laterItems.FirstOrDefault(x => x.Title.StartsWith(searchLetter, StringComparison.InvariantCultureIgnoreCase));
            }
            if (thingToFind == null)
                return;
            lb.ScrollIntoView(thingToFind);
            cv.MoveCurrentTo(thingToFind);
        }

        private void LbOverlays_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ListBox lb) || e.Key < Key.A || e.Key > Key.Z)
                return;

            string searchLetter = e.Key.ToString();
            ObservableCollection<IconOverlay> Things = (ObservableCollection<IconOverlay>)lb.ItemsSource;
            ICollectionView cv = CollectionViewSource.GetDefaultView(Things);
            IconOverlay thingToFind;
            if (lb.SelectedItem == null)
            {
                thingToFind = Things.FirstOrDefault(x => x.Title.StartsWith(searchLetter, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                int currentIndex = lb.Items.IndexOf((IconOverlay)lb.SelectedItem);
                List<IconOverlay> laterItems = Things.Skip(currentIndex + 1).ToList();
                thingToFind = laterItems.FirstOrDefault(x => x.Title.StartsWith(searchLetter, StringComparison.InvariantCultureIgnoreCase));
            }
            if (thingToFind == null)
                return;
            lb.ScrollIntoView(thingToFind);
            cv.MoveCurrentTo(thingToFind);
        }

        private void LbIcons_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model.SelectedIcons = lbIcons.SelectedItems.OfType<Icon>().ToArray();
        }

        private void LbOverlays_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _model.SelectedOverlays = lbOverlays.SelectedItems.OfType<IconOverlay>().ToArray();
        }

        private void OnOverlayPositionClick(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton && toggleButton.Tag is OverlayPosition overlayPosition)
            {
                _model.SuspendPreview();
                if (overlayPosition == OverlayPosition.None)
                    lbOverlays.SelectedItems.Clear();

                _model.OverlayPosition = overlayPosition;
                _model.ResumePreview();
            }
        }
    }
}
