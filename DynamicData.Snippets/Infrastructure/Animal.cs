using System.Collections.Generic;
using DynamicData.Binding;

namespace DynamicData.Snippets.Infrastructure
{
    public enum AnimalFamily
    {
        Mammal,
        Reptile,
        Fish,
        Amphibian,
        Bird
    }



    public class Animal: AbstractNotifyPropertyChanged
    {

        public string Name { get; }
        public string Type { get; }
        public AnimalFamily Family { get; }


        private bool _includeInResults;
        public bool IncludeInResults
        {
            get => _includeInResults;
            set => SetAndRaise(ref _includeInResults, value);
        }


        public Animal(string name, string type, AnimalFamily family)
        {
            Name = name;
            Type = type;
            Family = family;
        }
    }
}
