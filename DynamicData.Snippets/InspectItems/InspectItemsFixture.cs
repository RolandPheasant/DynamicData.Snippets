using System.Linq;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicData.Snippets.InspectItems
{
    [TestFixture]
    public class InspectItemsFixture
    {
        [Test]
        public void InspectCollection()
        {
            var items = new[] {"A", "A", "B", "C"}
                .Select((str, index) => new SimpleImmutableObject(index, str))
                .ToArray();

            using (var sourceList = new SourceList<SimpleImmutableObject>())
            using (var sut = new InspectCollection(sourceList))
            {
                sourceList.AddRange(items);

                sut.DistinctCount.Should().Be(3);
                sut.Count.Should().Be(4);

                sourceList.RemoveAt(0);

                sut.DistinctCount.Should().Be(3);
                sut.Count.Should().Be(3);

            }
        }

        [Test]
        public void InspectCollectionWithPropertyChanges()
        {
            var initialItems = Enumerable.Range(1, 10)
                .Select(i => new SimpleNotifyPropertyChangedObject(i))
                .ToArray();

            using (var sourceList = new SourceList<SimpleNotifyPropertyChangedObject>())
            using (var sut = new InspectCollectionWithPropertyChanges(sourceList))
            {
                sourceList.AddRange(initialItems);

                //check values are set when collection is loaded
                sut.AllActive.Should().Be(false);
                sut.AllInActive.Should().Be(true);
                sut.Count.Should().Be(10);

                //change some properties and check aggregated values
                initialItems[1].IsActive = true;
                sut.AllInActive.Should().Be(false);

                foreach (var item in initialItems)
                    item.IsActive = true;

                sut.AllActive.Should().Be(true);
                sut.AllInActive.Should().Be(false);

                // change the underlying collection
                sourceList.RemoveRange(0, 5);

                sut.AllActive.Should().Be(true);
                sut.AllInActive.Should().Be(false);
                sut.Count.Should().Be(5);

            }
        }

        [Test]
        public void InspectCollectionWithObservable()
        {
            var initialItems = Enumerable.Range(1, 10)
                .Select(i => new SimpleObjectWithObservable(i))
                .ToArray();
            
            //result should only be true when all items are set to true

            using (var sourceList = new SourceList<SimpleObjectWithObservable>())
            using (var sut = new InspectCollectionWithObservable(sourceList))
            {

                sourceList.AddRange(initialItems);
                sut.AllActive.Should().Be(false);

                //should remain false because
                initialItems[0].SetIsActive(true);
                sut.AllActive.Should().Be(false);

                //set all items to true
                foreach (var item in initialItems) item.SetIsActive(true);
                sut.AllActive.Should().Be(true);

                initialItems[0].SetIsActive(false);
                sut.AllActive.Should().Be(false);

                sourceList.Clear();
                sut.AllActive.Should().Be(false);
            }
        }
    }
}
