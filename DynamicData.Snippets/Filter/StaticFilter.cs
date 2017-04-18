using System;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Filter
{
    public class StaticFilter: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public IObservableList<Animal> Mammals { get; }

        public StaticFilter(IObservableList<Animal> source)
        {
            //this list will automatically filter by Mammals only when the underlying list receives adds, or removes
            Mammals = source.Connect()
                .Filter(animal => animal.Family == AnimalFamily.Mammal)
                .AsObservableList();
            
            _cleanUp = Mammals;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}