using System;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Sorting
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomBinding: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public  ReadOnlyObservableCollection<Animal> Data { get; }

        public CustomBinding(IObservableCache<Animal, string> source)
        {
            /*
                 Sometimes the default binding does not behave exactly as you want.
                 Using VariableThresholdObservableCollectionAdaptor is an example of how you can inject your own behaviour.
            */

            Threshold = 5;

            _cleanUp = source.Connect()
                .Sort(SortExpressionComparer<Animal>.Ascending(a => a.Name))
                .Bind(out var data, adaptor: new VariableThresholdObservableCollectionAdaptor<Animal, string>(() => Threshold))
                .Subscribe();

            Data = data;
        }

        public int Threshold { get; set; } 

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    /// <summary>
    /// This is cloned from DynamicData and has been changed so the reset threshold can be adjusted
    /// </summary>
    public class VariableThresholdObservableCollectionAdaptor<TObject, TKey> : ISortedObservableCollectionAdaptor<TObject, TKey>
    {
        private readonly Func<int> _refreshThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="refreshThreshold">The number of changes before a Reset event is used</param>
        public VariableThresholdObservableCollectionAdaptor(Func<int> refreshThreshold)
        {
            _refreshThreshold = refreshThreshold;
        }

        /// <summary>
        /// Maintains the specified collection from the changes
        /// </summary>
        /// <param name="changes">The changes.</param>
        /// <param name="collection">The collection.</param>
        public void Adapt(ISortedChangeSet<TObject, TKey> changes, IObservableCollection<TObject> collection)
        {
            switch (changes.SortedItems.SortReason)
            {
                case SortReason.InitialLoad:
                {
                    if (changes.Count > _refreshThreshold())
                    {
                        using (collection.SuspendNotifications())
                        {
                            collection.Load(changes.SortedItems.Select(kv => kv.Value));
                        }
                    }
                    else
                    {
                        using (collection.SuspendCount())
                        {
                            DoUpdate(changes, collection);
                        }
                    }
                    }
                    break;
                case SortReason.ComparerChanged:
                case SortReason.Reset:
                    using (collection.SuspendNotifications())
                    {
                        collection.Load(changes.SortedItems.Select(kv => kv.Value));
                    }
                    break;

                case SortReason.DataChanged:
                    if (changes.Count > _refreshThreshold())
                    {
                        using (collection.SuspendNotifications())
                        {
                            collection.Load(changes.SortedItems.Select(kv => kv.Value));
                        }
                    }
                    else
                    {
                        using (collection.SuspendCount())
                        {
                            DoUpdate(changes, collection);
                        }
                    }
                    break;

                case SortReason.Reorder:
                    //Updates will only be moves, so appply logic
                    using (collection.SuspendCount())
                    {
                        DoUpdate(changes, collection);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoUpdate(ISortedChangeSet<TObject, TKey> updates, IObservableCollection<TObject> list)
        {
            foreach (var update in updates)
            {
                switch (update.Reason)
                {
                    case ChangeReason.Add:
                        list.Insert(update.CurrentIndex, update.Current);
                        break;
                    case ChangeReason.Remove:
                        list.RemoveAt(update.CurrentIndex);
                        break;
                    case ChangeReason.Moved:
                        list.Move(update.PreviousIndex, update.CurrentIndex);
                        break;
                    case ChangeReason.Update:
                        list.RemoveAt(update.PreviousIndex);
                        list.Insert(update.CurrentIndex, update.Current);
                        break;
                }
            }
        }
    }
}
