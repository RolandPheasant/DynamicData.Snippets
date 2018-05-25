using System.Reactive.Subjects;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Virtualise
{
	public class PagingListWithVirtualiseFixture
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
            new Animal("Sharon", "Red Backed Shrike", AnimalFamily.Bird)
        };

		[Fact]
		public void PagingListWithVirtualise()
		{
			using (var pager = new BehaviorSubject<IVirtualRequest>(new VirtualRequest(0, 0)))
			using (var sourceList = new SourceList<Animal>())
			using (var sut = new PagingListWithVirtualise(sourceList, pager))
			{
				// Add items to source
				sourceList.AddRange(_items);

				// Requested 0 items, so no data should be returned
				sut.Virtualised.Count.Should().Be(0);

                // Requested 2 items starting at position 0, 2 items expected
				pager.OnNext(new VirtualRequest(0, 2));
				sut.Virtualised.Count.Should().Be(2);

				// Requested 2 items starting at position 2, 2 items expected
				pager.OnNext(new VirtualRequest(2, 2));
				sut.Virtualised.Count.Should().Be(2);

				// Requested 5 items starting at position 0, 5 items expected
				pager.OnNext(new VirtualRequest(0, 5));
				sut.Virtualised.Count.Should().Be(5);

				// Requested 1 item starting at position 5, 1 items expected
				pager.OnNext(new VirtualRequest(5, 1));
				sut.Virtualised.Count.Should().Be(1);
			}
		}
	}
}
