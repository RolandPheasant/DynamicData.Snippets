using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Watch
{
    public class SelectCacheItem: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IDisposable _cleanUp;
        private TransformedCacheItem _selectedItem;

        private readonly  ISourceCache<CacheItem, string> _sourceCache = new SourceCache<CacheItem, string>(ci=>ci.Id);
        private readonly IObservableCache<TransformedCacheItem, string> _transformedCache;
        private readonly SerialDisposable _autoSelector = new SerialDisposable();

        public ReadOnlyObservableCollection<TransformedCacheItem> Data { get; }

        public SelectCacheItem(ISchedulerProvider schedulerProvider)
        {
            /*
             * Example to show how to select an item after after it has been added to a cache and subsequently transformed.
             *
             * If a filter is applied before the transform this methodology will not work. In that case, an alternative would be
             * to add .Do(changes => some custom logic) after the bind statement
             */

            _schedulerProvider = schedulerProvider;

            _transformedCache = _sourceCache.Connect()
                .Transform(si => new TransformedCacheItem(si))
                .AsObservableCache();

            var binder = _transformedCache.Connect()
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out var data)
                .Subscribe();

            Data = data;

            _cleanUp = new CompositeDisposable(binder, _transformedCache, _sourceCache);
        }

        public void Load(CacheItem[] items)
        {
            _sourceCache.AddOrUpdate(items);

            SelectWhenLoaded(items[0].Id);
        }

        public void AddOrUpdate(CacheItem item)
        {
            _sourceCache.AddOrUpdate(item);

            SelectWhenLoaded(item.Id);
        }

        private void SelectWhenLoaded(string id)
        {
            _autoSelector.Disposable = _transformedCache.Watch(id)
                .ObserveOn(_schedulerProvider.MainThread) //put the code onto the main thread so it happens after binding
                .Subscribe(change => SelectedItem = change.Current);

        }

        public TransformedCacheItem SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(ref _selectedItem, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }


    public class CacheItem
    {
        public string Id { get; }

        public CacheItem(string id)
        {
            Id = id;
        }
    }

    public class TransformedCacheItem
    {
        public string Id { get; }

        public TransformedCacheItem(CacheItem cacheItem)
        {
            Id = cacheItem.Id;
            Description = $"Transformed {Id}";
        }

        public string Description { get; }
    }



}
