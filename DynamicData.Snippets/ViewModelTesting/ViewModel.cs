using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.ViewModelTesting
{
    public class ViewModel: AbstractNotifyPropertyChanged, IDisposable
    {
        public ReadOnlyObservableCollection<ItemViewModel> BindingData { get; }

        private readonly IDisposable _cleanUp;
        private bool _isPaused;
        private bool _showEmptyView;

        public ViewModel(IDataProvider dataProvider, ISchedulerProvider schedulerProvider)
        {
            var paused = this.WhenValueChanged(vm => vm.IsPaused);

            /*
             * NB: When ObserveOn is required, or asynchronous threads are introduced, always paramatise the threads via a scheduler provider 
             * or some other means of achieving the same thing
             */

              var dataLoader = dataProvider.ItemCache
                .Connect()
                .Transform(CreateItemViewModel)             
                .BatchIf(paused,  schedulerProvider.Background) 
                .Sort(SortExpressionComparer<ItemViewModel>.Descending(i => i.Item.Id), resetThreshold: 20)
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out var bindingData)
                .Subscribe();

            BindingData = bindingData;

            var counter =  dataProvider.ItemCache.CountChanged.Subscribe(i => ShowEmptyView = i == 0);

            _cleanUp = new CompositeDisposable(dataLoader, counter);
        }

        public bool ShowEmptyView
        {
            get { return _showEmptyView; }
            set { SetAndRaise(ref _showEmptyView, value); }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set { SetAndRaise(ref _isPaused, value);}
        }

        private ItemViewModel CreateItemViewModel(Item item)
        {
            return new ItemViewModel(item);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }



}