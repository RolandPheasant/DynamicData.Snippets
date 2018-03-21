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

        public SimplePagging(IObservableList<Animal> source, IObservable<IPageRequest> pager)
        {
            Paged = source.Connect()
                .Page(pager)
                .Do(changes => Console.WriteLine(changes.TotalChanges), ex => Console.WriteLine(ex)) //added as a quick and dirty way to debug
                .AsObservableList();

            _cleanUp = Paged;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
