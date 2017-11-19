using System;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.AutoRefresh
{
    public class AutoRefreshForPropertyChanges
    {

        public IObservable<int> DistinctCount { get; }

        public AutoRefreshForPropertyChanges(ISourceCache<MutableThing, int> dataSource)
        {
            /*
             * The observable cache has no concept of mutable properties. It only knows about adds, updates and removes.
             * However it does have the concept of a refresh, which is a manual way to tell downstream operators like 
             * distinct, sort, filter and grouping to re-evaluate 
             * 
             * To force a refresh, you need to manually trigger when to do so. In this case property changes are monitored
             * and the data source sends a refresh signal to all downstream operators.
             */
            DistinctCount = dataSource.Connect()
                .AutoRefresh(t=>t.Value) //Omit param to refresh for any property
                .DistinctValues(m => m.Value)
                .Count();
        }

        public AutoRefreshForPropertyChanges(IObservable<IChangeSet<MutableThing, int>> dataSource)
        {
            /* 
             * As per above example but in cases where the observable cache is not available 

             */

            DistinctCount = dataSource
                .AutoRefresh(t => t.Value)
                .DistinctValues(m => m.Value)
                .Count();
        }
    }

    public class MutableThing : AbstractNotifyPropertyChanged
    {
        private string _value;

        public MutableThing(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public int Id { get; }

        public string Value
        {
            get => _value;
            set => SetAndRaise(ref _value, value);
        }
    }
}