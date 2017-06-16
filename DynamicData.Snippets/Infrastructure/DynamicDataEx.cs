using System;
using DynamicData.Aggregation;

namespace DynamicData.Snippets.Infrastructure
{
    public static class DynamicDataEx
    {
        public static IObservable<IChangeSet<TObject, TKey>> ExcludeSameReferenceUpdates<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source)
        {
            return source.IgnoreUpdateWhen((current, previous) => ReferenceEquals(current, previous));
        }
        
        public static IObservable<int> Count<TObject>(this IObservable<IDistinctChangeSet<TObject>> source)
        {
            return source.ForAggregation().Count();
        }
    }
}