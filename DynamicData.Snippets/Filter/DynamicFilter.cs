using System;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Filter
{
    public class DynamicFilter: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp ;
        private string _animalFilter;

        public IObservableList<Animal> Filtered { get; }

        public DynamicFilter(IObservableList<Animal> source, ISchedulerProvider  schedulerProvider)
        {
            //produce an observable which creates a new predicate whenever AnimalFilter property changes
            var dynamicFilter = this.WhenValueChanged(@this => @this.AnimalFilter)
                .Throttle(TimeSpan.FromMilliseconds(250), schedulerProvider.Background) //throttle to prevent constant filtering (i.e. when users type)
                .Select(CreatePredicate);

            //Create list which automatically filters when AnimalFilter changes
            Filtered = source.Connect()
                .Filter(dynamicFilter) //dynamicfilter can accept any predicate observable (i.e. does not have to be based on a property)
                .AsObservableList();

            _cleanUp = Filtered;
        }
        
        public string AnimalFilter
        {
            get => _animalFilter;
            set => SetAndRaise(ref _animalFilter, value);
        }

        private Func<Animal, bool> CreatePredicate(string text)
        {
            if (text == null || text.Length < 3)
                return animal => true;

            //the more fields which are filtered on the slower it takes for the filter to apply by generally I have never found checking a predicate to be particularly slow 
            return animal => animal.Name.Contains(text)
                             || animal.Type.Contains(text)
                             || animal.Family.ToString().Contains(text);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
