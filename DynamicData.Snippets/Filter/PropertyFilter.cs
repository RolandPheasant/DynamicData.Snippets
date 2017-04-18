using System;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Filter
{
    public class PropertyFilter : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        public IObservableList<Animal> Filtered { get; }

        public PropertyFilter(IObservableList<Animal> source, ISchedulerProvider schedulerProvider)
        {
            /*
             *  Create list which automatically filters:
             *  
             * a) When the underlying list changes
             * b) When IncludeInResults property changes 
             * c) NB: Add throttle when IncludeInResults properties can change in multiple animals in quick sucession 
             *      (i.e. each time the prop changes the filter is re-assessed potentially leading to a flurry of updates - better to slow that down)
             */

            Filtered = source.Connect()
                .FilterOnProperty(animal => animal.IncludeInResults, animal => animal.IncludeInResults, TimeSpan.FromMilliseconds(250), schedulerProvider.Background)
                .AsObservableList();

            _cleanUp = Filtered;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}