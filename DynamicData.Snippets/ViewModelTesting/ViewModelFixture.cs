using System;
using System.Linq;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicData.Snippets.ViewModelTesting
{
    [TestFixture]
    public class ViewModelFixture
    {
        [Test]
        public void Binding()
        {
            var schedulerProvider = new TestSchedulerProvider();
            using (var testData = new DataProviderStub())
            using (var sut = new ViewModel(testData, schedulerProvider))
            {
                //Act
                var items = Enumerable.Range(1, 10).Select(i => new Item(i)).ToArray();
                testData.Data.AddOrUpdate(items);
                schedulerProvider.TestScheduler.Start(); //push scheduler forward

                //1. Check count of data
                sut.BindingData.Count.Should().Be(10);

                //2. Check Transform and Sort
                var expectedData = items
                    .Select(i => new ItemViewModel(i))
                    .OrderByDescending(vm => vm.Item.Id);

                sut.BindingData.ShouldAllBeEquivalentTo(expectedData);
            }
        }

        [Test]
        public void IsPaused()
        {
            var schedulerProvider = new TestSchedulerProvider();
            using (var testData = new DataProviderStub())
            using (var sut = new ViewModel(testData, schedulerProvider))
            {
                sut.IsPaused = true;
                schedulerProvider.TestScheduler.Start(); //push scheduler forward

                //add data after pause has started
                testData.Data.AddOrUpdate(Enumerable.Range(1, 10).Select(i => new Item(i)));
                schedulerProvider.TestScheduler.AdvanceBy(1);

                //check no data has been pipelined
                sut.BindingData.Count.Should().Be(0);

                //turn pause off and check the updates have been pushed through
                sut.IsPaused = false;
                schedulerProvider.TestScheduler.AdvanceBy(1);
                sut.BindingData.Count.Should().Be(10);
            }
        }


        private class DataProviderStub : IDataProvider, IDisposable
        {
            //create a backend data source for our tests
            public ISourceCache<Item, int> Data { get; } = new SourceCache<Item, int>(i => i.Id);

            private readonly IObservableCache<Item, int> _itemCache;
            IObservableCache<Item, int> IDataProvider.ItemCache => _itemCache;

            public DataProviderStub()
            {
                _itemCache = Data.AsObservableCache();
            }

            public void Dispose()
            {
                _itemCache.Dispose();
            }
        }
    }
}
