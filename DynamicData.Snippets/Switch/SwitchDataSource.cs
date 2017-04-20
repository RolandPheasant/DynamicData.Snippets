using System;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace DynamicData.Snippets.Switch
{
    public enum SwitchDataSourceOption
    {
        SourceA,
        SourceB
    }

    public sealed class SwitchDataSource: AbstractNotifyPropertyChanged, IDisposable
    {
        private SwitchDataSourceOption _option;
        private readonly IDisposable _cleanUp;

        public IObservableList<int> DataSource { get; }

        public SwitchDataSource(IObservableList<int> sourceA, IObservableList<int> sourceB)
        {
            /*
             * Switching data acts on IObservable<IChangSet<T>> or IObservable<IObservableList<T>>
             * 
             * The same concept applies to the ObservableCache
             */

            DataSource = this.WhenValueChanged(@this => @this.Option)
                .Select(opt => opt == SwitchDataSourceOption.SourceA ? sourceA : sourceB)
                .Switch()
                .AsObservableList();

            _cleanUp = DataSource;
        }
        
        public SwitchDataSourceOption Option
        {
            get => _option;
            set => SetAndRaise(ref _option, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
