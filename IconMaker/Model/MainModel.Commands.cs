using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using IconMaker.wpf;

namespace IconMaker.Model
{
    public partial class MainModel
    {
        public RelayCommand CmdNew { get; private set; }
        public RelayCommand CmdOpen { get; private set; }
        public RelayCommand CmdSave { get; private set; }
        public RelayCommand CmdPrevIcon { get; private set; }
        public RelayCommand CmdNextIcon { get; private set; }
        public RelayCommand CmdAddToCollection { get; private set; }
        public RelayCommand CmdCloseCollection { get; private set; }
        public RelayCommand CmdModifyColor { get; private set; }

        private void InitCommands()
        {
            CmdNew = new RelayCommand(OnNew);
            CmdOpen = new RelayCommand(OnOpen);
            CmdSave = new RelayCommand(OnSave, CanSave);
            CmdPrevIcon = new RelayCommand(OnPrevIcon, CanPrevIcon);
            CmdNextIcon = new RelayCommand(OnNextIcon, CanNextIcon);
            CmdAddToCollection = new RelayCommand(OnAddToCollection, CanAddToCollection);
            CmdCloseCollection = new RelayCommand(OnCloseCollection);
            CmdModifyColor = new RelayCommand(OnModifyColor, CanModifyColor);
        }

        public IList SelectedColorMapEntries { get; set; } = new List<ColorMapEntry>();

        private void OnModifyColor(object obj)
        {
        }

        private bool CanModifyColor(object arg)
        {
            return SelectedColorMapEntries.Count > 0;
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
                CurrentCollection.Icons.Add(new CollectionIcon(viewbox) {Title = title});
            }
        }

        private bool CanAddToCollection(object arg)
        {
            return CurrentCollection != null && SelectedCount > 0;
        }

        private void OnPrevIcon(object obj)
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
        }

        private bool CanPrevIcon(object arg)
        {
            return SelectedIndex > 0;
        }

        private void OnNextIcon(object obj)
        {
            SelectedIndex = Math.Min(SelectedCount - 1, SelectedIndex + 1);
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
    }
}