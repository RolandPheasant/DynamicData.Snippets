
using System.Collections.Generic;
using System.Linq;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicData.Snippets.Group
{
    [TestFixture]
    public class GroupFixture
    {
        [Test]
        public void GroupAndMonitorPropertyChanges()
        {
            //create manual grouping so we can guage expectations
            IEnumerable<SpeciesGroup> ManualGrouping(ISourceList<Species> items)
            {
                return items.Items.GroupBy(i => i.Name[0]).Select(g => new SpeciesGroup(g.Key, g.ToArray()));
            }

            using (var sourceList = new SourceList<Species>())
            using (var grouper = new GroupAndMonitorPropertyChanges(sourceList))
            {
                //populate with initial data
                var initialData = new[] { new Species("Ant"), new Species("Ape"), new Species("Bear"), new Species("Boar"), new Species("Cougar")};
                sourceList.AddRange(initialData);
                
                //Check all data has loaded
                grouper.SpeciesByLetter.Items.SelectMany(g => g.Items).ShouldAllBeEquivalentTo(initialData);
                grouper.SpeciesByLetter.Items.ShouldBeEquivalentTo(ManualGrouping(sourceList));
                grouper.SpeciesByLetter.Count.Should().Be(3);

                //change the first letter of the data and the groupings will change
                initialData[0].Name = "Ánt"; //change the first letter
                grouper.SpeciesByLetter.Count.Should().Be(4);

                //assert everything
                grouper.SpeciesByLetter.Items.ShouldBeEquivalentTo(ManualGrouping(sourceList));
            }
        }

        [Test]
        public void XamarinGrouping()
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
            using (var grouper = new XamarinGrouping(sourceList, schedulerProvider))
            {
                //populate with initial data
                sourceList.AddRange(items);
                schedulerProvider.TestScheduler.Start();

                grouper.FamilyGroups.Count.Should().Be(5);
                grouper.FamilyGroups.Single(group => group.Family == AnimalFamily.Mammal).Count.Should().Be(5);
                grouper.FamilyGroups.Single(group => group.Family == AnimalFamily.Fish).Count.Should().Be(1);

                //apply a filter
                grouper.Filter = a => a.Type == "Dog" || a.Type == "Fish";

                schedulerProvider.TestScheduler.Start();
                grouper.FamilyGroups.Count.Should().Be(2);
                grouper.FamilyGroups.Single(group => group.Family == AnimalFamily.Mammal).Count.Should().Be(2);
                grouper.FamilyGroups.Single(group => group.Family == AnimalFamily.Fish).Count.Should().Be(1);

                //clear list and all groupings are gone
                sourceList.Clear();
                schedulerProvider.TestScheduler.Start();
                grouper.FamilyGroups.Count.Should().Be(0);
            }
        }

    }
}