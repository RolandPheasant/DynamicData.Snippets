using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace DynamicData.Snippets.Group
{

    public class GroupAndMonitorPropertyChanges
    {

        public GroupAndMonitorPropertyChanges()
        {
            var sourceList  = new SourceList<Species>();
            sourceList.AddRange(new[] {new Species("Ant"),new Species("Ape"),new Species("Bear"),new Species("Boar"),new Species("Cougar")});
            
            //share so we are not duplicating change sets
            var shared = sourceList.Connect().Publish();

            //fired when the name changes on any item in the source collection
            var nameChanged = shared.WhenValueChanged(species => species.Name);

            //group by first letter and pass in the nameChanged observable (as a unit) to instruct the grouping to re-apply the grouping
            var derivedGroupedList = shared
                .GroupWithImmutableState(x => x.Name[0], nameChanged.Select(_=> Unit.Default))
                .Transform(grouping => new SpeciesGroup(grouping.Key, grouping.Items))
                .AsObservableList();

            //*** Herein if the name of any of the species change the derived list will self maintain

             //Nothing happens until a Published source is connect
             var connected = shared.Connect();

            var disposeAllThisStuff = new CompositeDisposable(sourceList, derivedGroupedList, connected);
        }

        private class SpeciesGroup
        {
            public SpeciesGroup(char key, IEnumerable<Species> items)
            {
                this.Key = key;
                this.Items = items;
            }

            public IEnumerable<Species> Items { get; }

            public char Key { get; }
        }

        private class Species : AbstractNotifyPropertyChanged
        {
            public Species(string name)
            {
                Name = name;
            }

            private string _name;
            public string Name
            {
                get { return _name; }
                set { SetAndRaise(ref _name, value); }
            }
        }
    }


}
