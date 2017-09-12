using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.InspectItems
{
    public class InspectCollectionWithPropertyChanges: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public InspectCollectionWithPropertyChanges(ISourceList<SimpleNotifyPropertyChangedObject> source)
        {
            /*
                Example to illustrate how to inspect an entire collection when properties change.
            */

            var shared = source.Connect().Publish();

            //refresh entire collection when properties change
            var statsAggregator = shared.AutoRefresh(vm => vm.IsActive)
                .ToCollection()
                .Select(items =>
                {
                    //produce a new result when the collection itself changes, or when IsActive changes
                    //(any result can be returned)
                    return new
                    {
                        AllActive = items.All(i => i.IsActive),
                        AllInActive = items.All(i => !i.IsActive),
                        AnyActive = items.Any(i => i.IsActive),
                        Count = items.Count,
                    };
                }).Subscribe(x =>
                {
                    AllActive = x.AllActive;
                    AllInActive = x.AllInActive;
                    AnyActive = x.AnyActive;
                    Count = x.Count;
                });

            _cleanUp = new CompositeDisposable(statsAggregator, shared.Connect());
        }
        
        public bool AllActive { get; set; }
        public bool AllInActive { get; set; }
        public bool AnyActive { get; set; }
        public int Count { get; set; }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class SimpleNotifyPropertyChangedObject : AbstractNotifyPropertyChanged
    {
        public int Id { get; }

        public SimpleNotifyPropertyChangedObject(int id)
        {
            Id = id;
        }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => SetAndRaise(ref _isActive, value);
        }
    }
}
