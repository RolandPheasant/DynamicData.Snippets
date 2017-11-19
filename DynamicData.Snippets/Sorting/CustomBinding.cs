using System;
using System.Collections.ObjectModel;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Sorting
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomBinding: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public  ReadOnlyObservableCollection<Animal> Data { get; }

        public CustomBinding(IObservableCache<Animal, string> source)
        {
            /*
                 Sometimes the default binding does not behave exactly as you want.
                 Using VariableThresholdObservableCollectionAdaptor is an example of how you can inject your own behaviour.
            */

            Threshold = 5;

            _cleanUp = source.Connect()
                .Sort(SortExpressionComparer<Animal>.Ascending(a => a.Name))
                .Bind(out var data, adaptor: new VariableThresholdObservableCollectionAdaptor<Animal, string>(() => Threshold))
                .Subscribe();

            Data = data;
        }

        public int Threshold { get; set; } 

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
