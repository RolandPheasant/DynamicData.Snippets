using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Filter
{

    public class ExternalSourceFilter : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        public IObservableList<Animal> Filtered { get; }

        public ExternalSourceFilter(IObservableList<Animal> source, IObservableList<AnimalFamily> families)
        {
            /*
             *  Create list which is filtered from the result of another filter
            */

            var familyFilter = families.Connect()
                .ToCollection()
                .Select(items =>
                {
                    bool Predicate(Animal animal) => items.Contains(animal.Family);
                    return (Func<Animal, bool>) Predicate;
                });

            Filtered = source.Connect()
                .Filter(familyFilter)
                .AsObservableList();

            _cleanUp = Filtered;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
