using System;
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
            /*
             *  The resulting data will match the exact page parameters specified
             *
             * 1. If you request new PageRequest(1, 5) you will get the first 5 items on page 1 
             * 2. If the next request is new PageRequest(1, 6) you will get the first 6 items on page 1
             *      [the second call will is clever enough to transmit a changeset with a single change as it does a diff set]
             *
             * 3. If you call new PageRequest(2,2) this changes the page to items on the new page and removes the old ones
             *
             * 4. If any changes take place to the underlaying data source, the current page will self-maintain
             */


            using (var pager = new BehaviorSubject<IPageRequest>(new PageRequest(0, 0)))
            using (var sourceList = new SourceList<Animal>())
            using (var sut = new SimplePagging(sourceList, pager))
            {
                // Add items to source
                sourceList.AddRange(_items);

                // No page was requested, so no data should be returned
                sut.Paged.Count.Should().Be(0);

                // Requested first 2 items from the underlying data
                pager.OnNext(new PageRequest(1, 2));
                sut.Paged.Count.Should().Be(2);

                // Requested first 4 items from the underlying data -> expect a changeset of 2
                pager.OnNext(new PageRequest(1, 4));
                sut.Paged.Count.Should().Be(4);

                // Requested first 4 items from page 2 the underlying data -> expect a changeset of 4
                pager.OnNext(new PageRequest(2, 4));
                sut.Paged.Count.Should().Be(4);

            }
        }
    }
}
