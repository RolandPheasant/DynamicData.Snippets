using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Creation
{
    public class CreationFixture
    {
        [Fact]
        public void ListFromTask()
        {
            using (var sut = ChangeSetCreation.FromTask().AsObservableList())
            {
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 10));
            }
        }

        [Fact]
        public void ListFromObservable()
        {
            var initial = new BehaviorSubject<IEnumerable<int>>(Enumerable.Range(1, 10));
            var subscriptions = new Subject<int>();

            using (var sut = ChangeSetCreation.FromObservable(initial, subscriptions).AsObservableList())
            {
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 10));

                subscriptions.OnNext(11);
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 11));
            }
        }

        [Fact]
        public void Reloadable()
        {
            var loader = new Subject<Unit>();
            int loadCount = 0;

            using (var sut = ChangeSetCreation.Reloadable(loader)
                .Do(changes=> loadCount++)
                .AsObservableList())
            {
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 10));
                loadCount.Should().Be(1);

                loader.OnNext(Unit.Default);
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 10));

                //the count will be 3 rather than 2 because a .Clear() is first called when an observable change set is switched
                loadCount.Should().Be(3);
            }
        }

        [Fact]
        public void LoadOnceWithRefcount()
        {
            int loadCount = 0;
            Task<IEnumerable<int>> Loader()
            {
                loadCount++;
                return Task.FromResult(Enumerable.Range(1, 10));
            }

            //Ref
            var refcountSource = ChangeSetCreation.FromTaskWithRefCount(Loader);


            using (var sut1 = refcountSource.AsObservableList())
            using (var sut2 = refcountSource.AsObservableList())
            {
                sut1.Count.Should().Be(10);
                sut2.Count.Should().Be(10);
            }
            loadCount.Should().Be(1);

            using (var sut1 = refcountSource.AsObservableList())
            using (var sut2 = refcountSource.AsObservableList())
            using (var sut3 = refcountSource.AsObservableList())
            using (var sut4 = refcountSource.AsObservableList())
            using (var sut5 = refcountSource.AsObservableList())
            {
                sut1.Count.Should().Be(10);
                sut2.Count.Should().Be(10);
                sut3.Count.Should().Be(10);
                sut4.Count.Should().Be(10);
                sut5.Count.Should().Be(10);
            }

            loadCount.Should().Be(2);
        }

        [Fact]
        public void ReloadableWithEditDiff()
        {
            var reloader = new Subject<Unit>();
            int loadCount = 0;
            IChangeSet<int> lastChangeSet = null;

            Task<IEnumerable<int>> Loader()
            {
                loadCount++;
                return Task.FromResult(loadCount == 1
                    ? Enumerable.Range(1, 10)
                    : Enumerable.Range(1, 12));
            }

            using (var sut = ChangeSetCreation.ReloadableWithEditDiff(reloader, Loader)
                .Do(changes => lastChangeSet = changes)
                .AsObservableList())
            {
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 10));
                loadCount.Should().Be(1);
                lastChangeSet.Adds.Should().Be(10);

                reloader.OnNext(Unit.Default);
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 12));
                lastChangeSet.Adds.Should().Be(2);
            }
        }

        [Fact]
        public void WithRetry()
        {
            int loadCount = 0;
            int failedCount = 0;

            Task<IEnumerable<int>> Loader()
            {
                loadCount++;

                if (loadCount < 3)
                {
                    failedCount++;
                    throw new Exception("Failed");
                }
                return Task.FromResult(Enumerable.Range(1, 10));
            }

            using (var sut = ChangeSetCreation.FromTask(Loader)
                .Retry(3) //in an enterprise app, would probably use a backoff retry strategy
                .AsObservableList())
            {
                sut.Items.Should().BeEquivalentTo(Enumerable.Range(1, 10));
                loadCount.Should().Be(3);
                failedCount.Should().Be(2);
            }
        }
    }
}
