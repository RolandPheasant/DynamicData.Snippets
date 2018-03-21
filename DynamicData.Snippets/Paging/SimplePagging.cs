using System;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData.Snippets.Infrastructure;

namespace DynamicData.Snippets.Paging
{
    public class SimplePagging : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        public IObservableList<Animal> Paged { get; }

        public IObservable<IPageRequest> PageRequests { get; set; } = Observable.Return(new PageRequest(0, 0));

        public SimplePagging(IObservableList<Animal> source)
        {
            Paged = source.Connect()
                .Page(PageRequests)
                .AsObservableList();

            _cleanUp = Paged;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
