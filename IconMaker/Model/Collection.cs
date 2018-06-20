using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IconMaker.Annotations;

namespace IconMaker.Model
{
    public class Collection : INotifyPropertyChanged
    {
        private static uint NextNumber;

        public Collection()
        {
            Title = $"Collection{++NextNumber}";
            Icons.CollectionChanged += (sender, args) =>
            {
                foreach (CollectionIcon collectionIcon in args.NewItems.OfType<CollectionIcon>())
                    collectionIcon.Owner = this;
            };
        }

        public string Title { get; set; }
        public SaveOptions SaveOptions { get; } = new SaveOptions();
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CollectionIcon> Icons { get; } = new ObservableCollection<CollectionIcon>();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
