
using System.Collections.Specialized;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicData.Snippets.Sorting
{
    [TestFixture]
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


        [Test]
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
    }
}
