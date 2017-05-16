using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;


namespace DynamicData.Snippets.Transform
{
    public class FlattenNestedObservableCollection: IDisposable
    {
        public IObservableCache<NestedChild, string> Children { get; }

        public FlattenNestedObservableCollection(IObservableCache<ClassWithNestedObservableCollection, int> source)
        {
            /*
             * Create a flat cache based on a nested observable collection
             */
            Children = source.Connect()
                        .TransformMany(parent => parent.Children.ToObservableChangeSet(c=>c.Name))
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
