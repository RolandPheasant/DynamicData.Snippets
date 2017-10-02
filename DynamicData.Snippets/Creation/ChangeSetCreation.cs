using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DynamicData.Snippets.Creation
{
    /// <summary>
    /// Examples of creating observable change sets for the observable list.
    /// 
    /// ObservableChangeSet.Create has exactly the same overloads as Observable.Create and has the same
    /// characteristics such as disposal of resources, error propagation and defered invocation i.e. when a subscription 
    /// or ToObservableList is invoked.
    /// 
    ///  Using these overloads makes observable changes sets first class observablea and thus enable repeat / retry logic
    /// 
    ///  I have ommitted cache examples as the signature is identical except the cache has a key specified
    /// eg var observable = ObservableChangeSet.Create<MyObject,int>(cache=>{}, p=>p.Id);
    /// </summary>
    public static class ChangeSetCreation
    {
        /// <summary>
        /// Reload data and maintain list using edit diff which calculates a diff set from the previous load which can significantly reduce noise by poreventing
        /// unnecessary updates
        /// </summary>
        public static IObservable<IChangeSet<int>> ReloadableWithEditDiff(IObservable<Unit> loadObservable, Func<Task<IEnumerable<int>>> loader)
        {
            return ObservableChangeSet.Create<int>(list =>
            {
                return loadObservable
                    .StartWith(Unit.Default) //ensure inital load
                    .SelectMany(_ => loader())
                    .Subscribe(items => list.EditDiff(items, EqualityComparer<int>.Default));
            });
        }

        /// <summary>
        /// Repeatedly reload data using Dynamic Data's Switch operator which will clear previous data
        /// and add newly loaded data
        /// </summary>
        public static IObservable<IChangeSet<int>> Reloadable(IObservable<Unit> loadObservable)
        {
            return loadObservable
                .StartWith(Unit.Default)
                .Select(_ => FromTask())
                .Switch();
        }


        /// <summary>
        /// Create an observable change set from a task
        /// </summary>
        /// <returns></returns>
        public static IObservable<IChangeSet<int>> FromTask()
        {
            return ObservableChangeSet.Create<int>(async list =>
            {
                var items = await LoadFromTask();
                list.AddRange(items);
                return () => { };
            });
        }

        public static IObservable<IChangeSet<int>> FromTask(Func<Task<IEnumerable<int>>> loader)
        {
            return ObservableChangeSet.Create<int>(async list =>
            {
                var items = await loader();
                list.AddRange(items);
                return () => { };
            });
        }

        /// <summary>
        /// Create an observable change set from 2 observables i) the initial load 2) a subscriber
        /// </summary>
        public static IObservable<IChangeSet<int>> FromObservable(IObservable<IEnumerable<int>> initialLoad, IObservable<int> subscriptions)
        {
            return ObservableChangeSet.Create<int>(list =>
            {
                //in an enterprise app, would have to account for the gap between load and subscribe
                var initialSubscriber = initialLoad
                    .Take(1)
                    .Subscribe(list.AddRange);

                var subscriber = subscriptions
                    .Subscribe(list.Add);

                return new CompositeDisposable(initialSubscriber, subscriber);
            });
        }

        private static Task<IEnumerable<int>> LoadFromTask()
        {
            return Task.FromResult(Enumerable.Range(1, 10));
        }
    }
}
