using System;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData.Aggregation;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Injection
{

    public class ForceRevalutationOfCacheOperators: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public IObservable<int> DistinctCount { get; }

        public ForceRevalutationOfCacheOperators(ISourceCache<MutableThing, int> dataSource)
        {
            /*
             * The observable cache has no concept of mutable properties. It only knows about adds, updates and removes.
             * However it does have the concept of an evaluate, which is a manual way to tell downstream operators like 
             * distinct, sort, filter and grouping to re-evaluate 
             * 
             * To force reevaluation, you need to manually trigger when to do so. In this case property changes are monitored
             * and the data source sends an evaluate signal to all downstream operators.
             */
             
            DistinctCount = dataSource.Connect()
                .DistinctValues(m => m.Value)
                .Do(x => { }, error => Console.WriteLine(error))
                .Count();

            _cleanUp = dataSource.Connect()
                .WhenPropertyChanged(m => m.Value,false)
                .Subscribe(_ => dataSource.Evaluate());
        }

        public ForceRevalutationOfCacheOperators(IObservable<IChangeSet<MutableThing, int>> dataSource)
        {
            /* 
             * As per above example but in cases where the observable cache is not available 
             * 
             *  NB: I created a new operator (See DynamicDataEx.cs in infrastructure folder which will be added to dynamic data in the next release)
             */

            var shared = dataSource.Publish();

            DistinctCount = shared
                .Evaluate(shared.WhenPropertyChanged(m => m.Value).Select(_=>Unit.Default))
                .DistinctValues(m => m.Value)
                .Count();

            _cleanUp = shared.Connect();
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }


    public class MutableThing:AbstractNotifyPropertyChanged
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
