using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;


namespace DynamicData.Snippets.InspectItems
{
    public class MonitorSelectedItems : IDisposable
    {
        private readonly ISourceList<SelectableItem> _source;
        private readonly IDisposable _cleanUp;

        public bool HasSelection { get; set; }
        public string SelectedMessage { get; set; }

        public MonitorSelectedItems(ISourceList<SelectableItem> source, MonitorSelectedItemsMode mode)
        {
            _source = source;

            //both methods produce the same result. However, UsingEntireCollection() enables producing values of selected and not-selected items
            _cleanUp = mode == MonitorSelectedItemsMode.UsingFilterOnProperty
                ? UseFilterOnProperty()
                : UseEntireCollection();
        }
        
        private IDisposable UseEntireCollection()
        {
            //produce an observable when the underlying list changes, or when IsSelected changes
            var shared = _source.Connect().Publish();
            var selectedChanged = shared.WhenPropertyChanged(si => si.IsSelected).ToUnit().StartWith(Unit.Default);
            var collectionChanged = shared.ToCollection().CombineLatest(selectedChanged, (items, _) => items).Publish();

            return new CompositeDisposable
                (
                    collectionChanged.Select(items => items.Any(si => si.IsSelected)).Subscribe(result => HasSelection = result),
                    collectionChanged.Select(items =>
                        {
                            var count = items.Count(si => si.IsSelected);
                            if (count == 0) return "Nothing Selected";
                            return count == 1 ? $"{count} item selected" : $"{count} items selected";
                        })
                        .Subscribe(result => SelectedMessage = result),
                    shared.Connect(),
                    collectionChanged.Connect()
                );
        }
    
        private IDisposable UseFilterOnProperty()
        {
            var selectedItems = _source.Connect()
                .FilterOnProperty(si => si.IsSelected, si => si.IsSelected)
                .ToCollection() //TODO:RP Implement optional bool parameter to specify whether to start with an empty result 
                .StartWith(new ReadOnlyCollection<SelectableItem>(new List<SelectableItem>()))
                .Publish();

            return new CompositeDisposable
            (
                selectedItems.Select(items => items.Any(si => si.IsSelected)).Subscribe(result => HasSelection = result),
                selectedItems.Select(items =>
                    {
                        var count = items.Count(si => si.IsSelected);
                        if (count == 0) return "Nothing Selected";
                        return count == 1 ? $"{count} item selected" : $"{count} items selected";
                    })
                    .Subscribe(result => SelectedMessage = result),
                selectedItems.Connect()
            );
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public enum MonitorSelectedItemsMode
    {
        UsingFilterOnProperty,
        UsingEntireCollection
    }

    public class SelectableItem : AbstractNotifyPropertyChanged
    {
        public int Id { get; }

        public SelectableItem(int id)
        {
            Id = id;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndRaise(ref _isSelected, value);
        }
    }
}