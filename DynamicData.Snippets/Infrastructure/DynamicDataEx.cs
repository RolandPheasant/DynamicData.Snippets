using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Aggregation;
using DynamicData.Binding;

namespace DynamicData.Snippets.Infrastructure
{
    public static class DynamicDataEx
    {
        /// <summary>
        /// Automatically refresh downstream operators when properties change.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source">The source observable</param>
        /// <param name="properties">Properties to observe. Specify one or more properties to monitor, otherwise leave blank </param>
        /// <param name="buffer">Apply a buffer period to batch up changes when the properties of one or more items change</param>
        /// <param name="scheduler">The scheduler</param>
        /// <returns></returns>
        public static IObservable<IChangeSet<TObject, TKey>> AutoRefresh<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source,
            IEnumerable<string> properties = null,
            TimeSpan? buffer=null, 
            IScheduler scheduler = null)
            where TObject: INotifyPropertyChanged
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.AutoRefresh((t, v) => t.WhenAnyPropertyChanged(properties?.ToArray() ?? new string[0]), buffer, scheduler);
        }

        public static IObservable<IChangeSet<TObject, TKey>> AutoRefresh<TObject, TKey, TAny>(this IObservable<IChangeSet<TObject, TKey>> source,
            Func<TObject, IObservable<TAny>> reevaluator,
            TimeSpan? buffer = null,
            IScheduler scheduler = null)
        {
            return source.AutoRefresh((t, v) => reevaluator(t), buffer, scheduler);
        }

        public static IObservable<IChangeSet<TObject, TKey>> AutoRefresh<TObject, TKey, TAny>(this IObservable<IChangeSet<TObject, TKey>> source, 
            Func<TObject, TKey, IObservable<TAny>> reevaluator,
            TimeSpan? buffer = null,
            IScheduler scheduler = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (reevaluator == null) throw new ArgumentNullException(nameof(reevaluator));

            return Observable.Create<IChangeSet<TObject, TKey>>(observer =>
            {
                var shared = source.Publish();

                //monitor each item observable and create change
                var changes = shared.MergeMany((t, k) =>
                {
                    return reevaluator(t, k).Select(_ => new Change<TObject, TKey>(ChangeReason.Evaluate, k, t));
                });

                //create a changeset, either buffered or one item at the time
                IObservable<IChangeSet<TObject, TKey>> refreshChanges;
                if (buffer == null)
                {
                    refreshChanges = changes.Select(c => new ChangeSet<TObject, TKey>(new[] { c }));
                }
                else
                {
                    refreshChanges = changes.Buffer(buffer.Value, scheduler ?? Scheduler.Default)
                        .Where(list => list.Any())
                        .Select(items => new ChangeSet<TObject, TKey>(items));
                }


                //publish refreshes and underlying changes
                var locker = new object();
                var publisher = shared.Synchronize(locker)
                    .Merge(refreshChanges.Synchronize(locker))
                    .SubscribeSafe(observer);

                return new CompositeDisposable(publisher, shared.Connect());
            });
        }


        public static IObservable<int> Count<TObject>(this IObservable<IDistinctChangeSet<TObject>> source)
        {
            return source.ForAggregation().Count();
        }

    }
}