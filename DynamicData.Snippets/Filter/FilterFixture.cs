using System.Linq;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Filter
{
    public class FilterFixture
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
        public void StaticFilter()
        {
            using (var sourceList = new SourceList<Animal>())
            using (var sut = new StaticFilter(sourceList))
            {
                sourceList.AddRange(_items);

                sut.Mammals.Items.ShouldAllBeEquivalentTo(_items.Where(a=>a.Family == AnimalFamily.Mammal));
                sut.Mammals.Count.Should().Be(5);

                //add a new mammal to show it is included in the result set
                sourceList.Add(new Animal("Bob","Human",AnimalFamily.Mammal));
                sut.Mammals.Count.Should().Be(6);

                //remove the first 4 items which will leave 2 mammals
                sourceList.RemoveRange(0,4);
                sut.Mammals.Count.Should().Be(2);
            }
        }

        [Fact]
        public void DynamicFilter()
        {
            var schedulerProvider = new TestSchedulerProvider();
            using (var sourceList = new SourceList<Animal>())
            using (var sut = new DynamicFilter(sourceList, schedulerProvider))
            {
                //start the scheduler
                schedulerProvider.TestScheduler.Start();

                //add items
                sourceList.AddRange(_items);

                sut.Filtered.Items.ShouldAllBeEquivalentTo(_items);
                sut.Filtered.Count.Should().Be(_items.Length);
                
                //set a filter 
                sut.AnimalFilter = "Dog";
                schedulerProvider.TestScheduler.Start();

                sut.Filtered.Items.ShouldAllBeEquivalentTo(_items.Where(a=>a.Type == "Dog"));
                sut.Filtered.Count.Should().Be(2);

                //add a new dog to show it is included in the result set
                sourceList.Add(new Animal("George", "Dog", AnimalFamily.Mammal));
                sut.Filtered.Count.Should().Be(3);

                //add a new bird to show it is included in the result set
                sourceList.Add(new Animal("Peter", "Parrot", AnimalFamily.Bird));
                sut.Filtered.Count.Should().Be(3);

                //My additions...
                sut.AnimalFilter = "Frog";
                schedulerProvider.TestScheduler.Start();
                sut.Filtered.Items.ShouldAllBeEquivalentTo(_items.Where(a => a.Type == "Frog"));
                sut.Filtered.Count.Should().Be(1);
            }
        }

        [Fact]
        public void PropertyFilter()
        {
            var schedulerProvider = new TestSchedulerProvider();
            using (var sourceList = new SourceList<Animal>())
            using (var sut = new PropertyFilter(sourceList, schedulerProvider))
            {
                //start the scheduler
                schedulerProvider.TestScheduler.Start();

                //add items
                sourceList.AddRange(_items);
                sut.Filtered.Count.Should().Be(0);

                //set to true to include in the result set
                _items[1].IncludeInResults = true;
                _items[2].IncludeInResults = true;
                _items[3].IncludeInResults = true;

                //progress scheduler 
                schedulerProvider.TestScheduler.Start();

                sut.Filtered.Items.ShouldAllBeEquivalentTo(new []{ _items[1] , _items[2] , _items[3] });
            }
        }
        
        [Fact]
        public void ExternalSourceFilter()
        {
            using (var sourceList = new SourceList<Animal>())
            using (var families = new SourceList<AnimalFamily>())
            using (var sut = new ExternalSourceFilter(sourceList, families))
            {
                //add items to source
                sourceList.AddRange(_items);
                sut.Filtered.Count.Should().Be(0);

                families.AddRange(new []{ AnimalFamily.Amphibian, AnimalFamily.Bird });
                
                sut.Filtered.Items.ShouldAllBeEquivalentTo(_items.Where(a => a.Family == AnimalFamily.Amphibian || a.Family==AnimalFamily.Bird));

                families.Remove(AnimalFamily.Amphibian);
                sut.Filtered.Items.ShouldAllBeEquivalentTo(_items.Where(a => a.Family == AnimalFamily.Bird));

            }
        }




    }
}
