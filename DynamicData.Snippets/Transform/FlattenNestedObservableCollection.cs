using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace DynamicData.Snippets.Transform
{
    public class FlattenNestedObservableCollection: IDisposable
    {
        public IObservableCache<NestedChild, string> Children { get; }

        public FlattenNestedObservableCollection(IObservableCache<ClassWithNestedObservableCollection, int> source)
        {
            /*
             * Create a flat cache based on a nested observable collection.
             * 
             * Since a new changeset is produced each time a parent is added, I recommend applying  Batch()
             * after TransformMany() to reduce notifications (particularly on initial load) 
             */
            Children = source.Connect()
                        .TransformMany(parent => parent.Children, c=>c.Name)
                        .AsObservableCache();
        }

     
        public void Dispose()
        {
            Children.Dispose();
        }
    }
    
    public class ClassWithNestedObservableCollection
    {
        public int Id { get; }
        public ObservableCollection<NestedChild> Children { get; }

        public ClassWithNestedObservableCollection(int id, IEnumerable<NestedChild> animals)
        {
            Id = id;
            Children = new ObservableCollection<NestedChild>(animals);
        }
    }

    public class NestedChild
    {
        public string Name { get; }
        public string Value { get; }

        public NestedChild(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
