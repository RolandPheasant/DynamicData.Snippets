using System.Reactive.Subjects;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Paging
{
    public class PagingFixture
    {
        private readonly Animal[] _items =
       {
            new Animal("Holly", "Cat", AnimalFamily.Mammal),
            new Animal("Rover", "Dog", AnimalFamily.Mammal),
            new Animal("Rex", "Dog", AnimalFamily.Mammal),
            new Animal("Whiskers", "Cat", AnimalFamily.Mammal),
            new Animal("Nemo", "Fish", AnimalFamily.Fish),
            new Animal("Moby Dick", "Whale", AnimalFamily.Mammal),
            new Animal("Fred", "Frog", AnimalFamily.Amphibian),
            new Animal("Isaac", "Next", AnimalFamily.Amphibian),
            new Animal("Sam", "Snake", AnimalFamily.Reptile),
            new Animal("Sharon", "Red Backed Shrike", AnimalFamily.Bird),
        };

        [Fact]
        public void SimplePagging()
        {
            using (var pager = new Subject<PageRequest>())
            using (var sourceList = new SourceList<Animal>())
            using (var sut = new SimplePagging(sourceList))
            {
                // Add items to source
                sourceList.AddRange(_items);
                sut.PageRequests = pager;

                // No page was requested, so no data should be returned
                sut.Paged.Count.Should().Be(0);

                // Requested a page with 2 items, so 2 items should be returned
                pager.OnNext(new PageRequest(1, 2));
                sut.Paged.Count.Should().Be(2);

                // Requested 2 more, so 4 items should be in the list
                pager.OnNext(new PageRequest(2, 2));
                sut.Paged.Count.Should().Be(4);

                // Requested 2 more, so 6 items should be in the list
                pager.OnNext(new PageRequest(3, 2));
                sut.Paged.Count.Should().Be(6);
            }
        }
    }
}
