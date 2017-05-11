using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Aggregation;

namespace DynamicData.Snippets.Infrastructure
{
    public static class DynamicDataEx
    {
        public static IObservable<IChangeSet<TObject, TKey>> Evaluate<TObject, TKey>(
            this IObservable<IChangeSet<TObject, TKey>> source, IObservable<Unit> reevaluator)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (reevaluator == null) throw new ArgumentNullException(nameof(reevaluator));

            return Observable.Create<IChangeSet<TObject, TKey>>(observer =>
            {
                var cache = new IntermediateCache<TObject, TKey>();
                return new CompositeDisposable(cache,
                    source.PopulateInto(cache),
                    cache.Connect().SubscribeSafe(observer),
                    reevaluator.Subscribe(_ => cache.Edit(innerCache => innerCache.Evaluate())));
            });
        }

        public static IObservable<int> Count<TObject>(this IObservable<IDistinctChangeSet<TObject>> source)
        {
            return source.ForAggregation().Count();
        }

    }
}