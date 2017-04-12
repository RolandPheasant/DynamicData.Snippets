using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.InspectItems
{
    public class InspectCollectionWithObservable : IDisposable
    {
        private readonly IDisposable _cleanUp;

        public InspectCollectionWithObservable(ISourceList<SimpleObjectWithObservable> source)
        {
            /*
                Example to illustrate how to inspect an entire collection and collate observable state.
            */

            //Capture the state of IsActive observable notification for each item in the collection
            var observableWithState = source.Connect().Transform(obj => new ObservableState(obj)).Publish();

            //fires an observable when any of the inner observables change
            var activeChanged = observableWithState.MergeMany(state => state.IsActive)
                .ToUnit()
                .StartWith(Unit.Default); //start with unit to ensure combine latest (below) yields when collection is loaded

            //Reveal the entire collecton when the underlying observable list changes i.e. adds, removes and replaces
            var collectionChanged = observableWithState.ToCollection();

            //combine latest collection and observable notifications and produce result indicating whether all items are Active
            IObservable<bool> areAllActive = collectionChanged.CombineLatest(activeChanged, (items, _) =>
            {
                return items.Any() && items.All(state => state.LatestValue.HasValue && state.LatestValue == true);
            });
            
            _cleanUp = new CompositeDisposable(areAllActive.Subscribe(allActive => AllActive = allActive), 
                observableWithState.Connect());
        }

        public bool AllActive { get; set; }

        private class ObservableState
        {
            public bool? LatestValue { get; private set; }
            public IObservable<bool> IsActive { get; }

            public ObservableState(SimpleObjectWithObservable source)
            {
                IsActive = source.IsActive.Do(value => LatestValue = value);
            }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
