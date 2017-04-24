using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Group
{
    public sealed class XamarinFormsGrouping: AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        public ReadOnlyObservableCollection<AnimalGroup> FamilyGroups { get; }
        
        public XamarinFormsGrouping(IObservableList<Animal> source, ISchedulerProvider schedulerProvider)
        {
            /* Xamarin forms is a bit dumb and cannot handle nested observable collections. 
             * To cirumvent this limitation, create a specialist observable collection with headers and use dynamic data to manage it */

            //create an observable predicate
            var observablePredicate = this.WhenValueChanged(@this => @this.Filter).ObserveOn(schedulerProvider.Background);

            _cleanUp = source.Connect()
                .Filter(observablePredicate)   //Apply filter dynamically
                .GroupOn(arg => arg.Family)                                             //create a dynamic group
                .Transform(grouping => new AnimalGroup(grouping, schedulerProvider))    //transform into a specialised observable collection
                .Sort(SortExpressionComparer<AnimalGroup>.Ascending(a => a.Family))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out var animals)      
                .DisposeMany()              //use DisposeMany() because the grouping is disposable
                .Subscribe();

            FamilyGroups = animals;
        }

        private Func<Animal, bool> _filter = a => true;
        public Func<Animal, bool> Filter
        {
            get => _filter;
            set => SetAndRaise(ref _filter, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    [DebuggerDisplay("{Header}")]
    public sealed class AnimalGroup : ObservableCollectionExtended<Animal>, IDisposable
    {
        private readonly IDisposable _cleanUp;

        public AnimalFamily Family { get; }

        public AnimalGroup(IGroup<Animal, AnimalFamily> grouping, ISchedulerProvider schedulerProvider)
        {
            this.Family = grouping.GroupKey;

            //load and sort the grouped list
            var dataLoader = grouping.List.Connect()
                .Sort(SortExpressionComparer<Animal>.Ascending(a => a.Name).ThenByAscending(a => a.Type))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(this, 2000) //make the reset threshold large because xamarin is slow when reset is called (or at least I think it is @erlend, please enlighten me )
                .Subscribe();

            //set the header when the group coount changes
            var headerSetter = grouping.List.CountChanged
                .Select(count => $"{grouping.GroupKey} - {count} items(s)")
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe(text => Header = text);

            _cleanUp = new CompositeDisposable(dataLoader, headerSetter);
        }

        string _header;
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Header)));
            }
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

}