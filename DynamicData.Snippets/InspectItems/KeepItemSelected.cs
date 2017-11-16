using System;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace DynamicData.Snippets.InspectItems
{
    public class KeepItemSelected : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private KeepSelectedObject _selectedItem;

        public KeepItemSelected(IObservableCache<KeepSelectedObject, int> source)
        {
            //an observable of updates to the selected item. When selecteditem == null, do nothing
            var selectedItemUpdates = this.WhenValueChanged(x => x.SelectedItem)
                .Select(selected => selected == null ? Observable.Never<KeepSelectedObject>() : source.WatchValue(selected.Id))
                .Switch();

            //when the item updates, reset the selected item. 
            _cleanUp = selectedItemUpdates.Subscribe(selected => SelectedItem = selected);
        }

        /// <summary>
        /// This is kept in sync with updates coming from the cache.
        /// </summary>
        public KeepSelectedObject SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(ref _selectedItem, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class KeepSelectedObject
    {
        public int Id { get; }
        public string Label { get; }

        public KeepSelectedObject(int id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}