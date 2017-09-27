
using System.Collections.Generic;
using System.Linq;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Group
{
    
    public class GroupFixture
    {
        [Fact]
        public void GroupAndMonitorPropertyChanges()
        {
            //create manual grouping so we can guage expectations
            IEnumerable<SpeciesGroup> ManualGrouping(ISourceList<Species> items)
            {
                return items.Items.GroupBy(i => i.Name[0]).Select(g => new SpeciesGroup(g.Key, g.ToArray()));
            }

            using (var sourceList = new SourceList<Species>())
            using (var sut = new GroupAndMonitorPropertyChanges(sourceList))
            {
                //populate with initial data
                var initialData = new[] { new Species("Ant"), new Species("Ape"), new Species("Bear"), new Species("Boar"), new Species("Cougar")};
                sourceList.AddRange(initialData);
                
                //Check all data has loaded
                sut.SpeciesByLetter.Items.SelectMany(g => g.Items).ShouldAllBeEquivalentTo(initialData);
                sut.SpeciesByLetter.Items.ShouldBeEquivalentTo(ManualGrouping(sourceList));
                sut.SpeciesByLetter.Count.Should().Be(3);

                //change the first letter of the data and the groupings will change
                initialData[0].Name = "Ánt"; //change the first letter
                sut.SpeciesByLetter.Count.Should().Be(4);

                //assert everything
                sut.SpeciesByLetter.Items.ShouldBeEquivalentTo(ManualGrouping(sourceList));
            }
        }

        [Fact]
        public void XamarinFormsGrouping()
        {
            var items = new[]
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

            var schedulerProvider = new TestSchedulerProvider();

            using (var sourceList = new SourceList<Animal>())
            using (var sut = new XamarinFormsGrouping(sourceList, schedulerProvider))
            {
                //populate with initial data
                sourceList.AddRange(items);
                schedulerProvider.TestScheduler.Start();

                sut.FamilyGroups.Count.Should().Be(5);
                sut.FamilyGroups.Single(group => group.Family == AnimalFamily.Mammal).Count.Should().Be(5);
                sut.FamilyGroups.Single(group => group.Family == AnimalFamily.Fish).Count.Should().Be(1);

                //apply a filter
                sut.Filter = a => a.Type == "Dog" || a.Type == "Fish";

                schedulerProvider.TestScheduler.Start();
                sut.FamilyGroups.Count.Should().Be(2);
                sut.FamilyGroups.Single(group => group.Family == AnimalFamily.Mammal).Count.Should().Be(2);
                sut.FamilyGroups.Single(group => group.Family == AnimalFamily.Fish).Count.Should().Be(1);

                //clear list and all groupings are gone
                sourceList.Clear();
                schedulerProvider.TestScheduler.Start();
                sut.FamilyGroups.Count.Should().Be(0);
            }
        }

        [Fact]
        public void CustomTotalRows()
        {
            const string USD = "USD";
            const string GBP = "GBP";
            const string CHF = "CHF";

            var items = new[]
            {
                new Trade(1, GBP, 100),
                new Trade(2, GBP, 200),
                new Trade(3, GBP, 100),
                new Trade(4, GBP, 200),
                new Trade(5, USD, 100),
                new Trade(6, USD, 200),
                new Trade(7, CHF, 100),
                new Trade(8, CHF, 200),
            };
            
            using (var source = new SourceCache<Trade, int>(t=> t.Id))
            using (var sut = new CustomTotalRows(source))
            {
                source.AddOrUpdate(items);

                //should be 1 grand total row, 3 sub total rows, 8 rows
                sut.AggregatedData.Count.Should().Be(12);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.GrandTotal).Should().Be(1);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.SubTotal).Should().Be(3);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.Item).Should().Be(8);
                sut.AggregatedData.Items.OrderBy(tp => tp.Ticker).ShouldAllBeEquivalentTo(CustomTotalRowsExpectation(source));

                //remove all gbp rows. should be 1 grand total row, 2 sub total rows, 4 rows
                source.RemoveKeys(new []{1,2,3,4});
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.GrandTotal).Should().Be(1);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.SubTotal).Should().Be(2);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.Item).Should().Be(4);
                sut.AggregatedData.Items.OrderBy(tp => tp.Ticker).ShouldAllBeEquivalentTo(CustomTotalRowsExpectation(source));
                
                //add a previously unseen ticker - a new sub total row should have been added
                source.AddOrUpdate(new Trade(100, "TRY", 2000));
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.GrandTotal).Should().Be(1);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.SubTotal).Should().Be(3);
                sut.AggregatedData.Items.Count(tp => tp.Key.Type == AggregationType.Item).Should().Be(5);
                sut.AggregatedData.Items.OrderBy(tp => tp.Ticker).ShouldAllBeEquivalentTo(CustomTotalRowsExpectation(source));

            }
        }

        private IEnumerable<TradeProxy> CustomTotalRowsExpectation(SourceCache<Trade, int> source)
        {
            yield return new TradeProxy(source.Items.ToArray(), new AggregationKey(AggregationType.GrandTotal, "All"));

            foreach (var proxy in source.Items
                .GroupBy(trade => new AggregationKey(AggregationType.Item, trade.Id.ToString()))
                .Select(g => new TradeProxy(g.ToArray(), g.Key)))
            {
                yield return proxy;
            }

            foreach (var proxy in source.Items
                .GroupBy(trade => new AggregationKey(AggregationType.SubTotal, trade.Ticker))
                .Select(g => new TradeProxy(g.ToArray(), g.Key)))
            {
                yield return proxy;
            }
        }


    }
}