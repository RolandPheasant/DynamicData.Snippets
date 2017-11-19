using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

//MUST INCLUDE NAMESPACE FOR AGGREGATIONS
using DynamicData.Aggregation;

namespace DynamicData.Snippets.Aggregation
{
    public class Aggregations : IDisposable
    {
        private readonly IDisposable _cleanUp;

        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Avg { get; set; }

        public Aggregations(IObservableList<int> source)
        {
            /*
             * Available aggregations: Max, Min, Avg, StdDev, Count, Sum.
             * 
             * For custom aggregations use: source.Connect().ToCollection().Select(items=>...);
             */
            var shared = source.Connect()
                //by default dd never notifies when the change set is empty i.e. upon subscripion when the source has no data
                //this means that no result is computed until data is loaded. However if you require a result even when the data source is empty, use StartWithEmpty() 
                .StartWithEmpty()
                //use standard rx Publish() / Connect() to share published changesets
                .Publish();

            _cleanUp = new CompositeDisposable
            (
                shared.Maximum(i => i).Subscribe(max => Max = max),
                shared.Minimum(i => i).Subscribe(min => Min = min),
                shared.Avg(i => i).Subscribe(avg => Avg = avg),

                shared.Connect()
            );
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
