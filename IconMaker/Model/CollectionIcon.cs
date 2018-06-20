using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using IconMaker.Annotations;
using PropertyChanged;

namespace IconMaker.Model
{
    public class CollectionIcon : INotifyPropertyChanged
    {
        private SaveOptions _saveOptions;
        public Collection Owner { get; internal set; }

        public CollectionIcon(Viewbox icon)
        {
            Icon = new Grid
            {
                Background = Brushes.Transparent,
                Children = { icon },
            };
        }

        public Grid Icon { get; }

        public string Title { get; set; }

        [AlsoNotifyFor(nameof(HasOwnSaveOptions))]
        public SaveOptions SaveOptions
        {
            get => _saveOptions ?? Owner?.SaveOptions;
            set => _saveOptions = value;
        }

        public bool HasOwnSaveOptions => _saveOptions != null;

        public SaveOptions OwnSaveOptions
        {
            get
            {
                if (_saveOptions == null)
                    SaveOptions = new SaveOptions(Owner?.SaveOptions);
                return SaveOptions;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
