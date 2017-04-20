using System;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace DynamicData.Snippets.Sorting
{
    public enum ChangeComparereOption
    {
        Ascending,
        Descending
    }

    public sealed class ChangeComparer : AbstractNotifyPropertyChanged, IDisposable
    {
        private ChangeComparereOption _option;
        private readonly IDisposable _cleanUp;

        public IObservableList<int> DataSource { get; }

        public ChangeComparer(IObservableList<int> source)
        {
            /*
             * Pass IObservable<IComparer<T>> into the sort operator to switch sorting
             * 
             * The same concept applies to the ObservableCache
             */

            var optionChanged = this.WhenValueChanged(@this => @this.Option)
                .Select(opt => opt == ChangeComparereOption.Ascending
                    ? SortExpressionComparer<int>.Ascending(i => i)
                    : SortExpressionComparer<int>.Descending(i => i));
            
            //create a sorted observable list
            DataSource = source.Connect()
                            .Sort(optionChanged)
                            .AsObservableList();

            _cleanUp = DataSource;
        }

        public ChangeComparereOption Option
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
