
using System;
using System.Collections.Specialized;
using System.Linq;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Sorting
{
    
    public class SortFixture
    {
        private readonly Animal[] _items = new[]
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
        public void CustomBinding()
        {
            // in this test we check whether the reset threshold can be dynamically controlled

            using (var sourceCache = new SourceCache<Animal, string>(a => a.Name))
            using (var sut = new CustomBinding(sourceCache))
            {
                int resetCount = 0;
                (sut.Data as INotifyCollectionChanged).CollectionChanged += (_, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Reset)
                        resetCount++;
                };

                sut.Threshold = 20;
                sourceCache.AddOrUpdate(_items);
                resetCount.Should().Be(0);

                sut.Threshold = 5;
                sourceCache.AddOrUpdate(_items);
                resetCount.Should().Be(1);

                sut.Threshold = 20;
                sourceCache.AddOrUpdate(_items);
                resetCount.Should().Be(1);
            }

        }

        [Fact]
        public void ChangeComparer()
        {
            const int size = 100;
            var randomValues = Enumerable.Range(1, size).OrderBy(_ => Guid.NewGuid()).ToArray();
            var ascending = Enumerable.Range(1, size).ToArray();
            var descending = Enumerable.Range(1, size).OrderByDescending(_ => Guid.NewGuid()).ToArray();

            using (var input = new SourceList<int>())
            using (var sut = new ChangeComparer(input))
            {
                input.AddRange(randomValues);

                sut.DataSource.Items.ShouldAllBeEquivalentTo(ascending);

                sut.Option = ChangeComparereOption.Descending;
                sut.DataSource.Items.ShouldAllBeEquivalentTo(descending);

                sut.Option = ChangeComparereOption.Ascending;
                sut.DataSource.Items.ShouldAllBeEquivalentTo(ascending);
            }
        }

    }
}
