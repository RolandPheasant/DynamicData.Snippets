using System;
using System.Collections.Generic;
using System.Linq;
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
            Func<ISourceList<Species>, IEnumerable<SpeciesGroup>> manualGrouping = items =>
            {
                return items.Items.GroupBy(i => i.Name[0]).Select(g => new SpeciesGroup(g.Key, g.ToArray()));
            };

            using (var sourceList = new SourceList<Species>())
            using (var grouper = new GroupAndMonitorPropertyChanges(sourceList))
            {
                //populate with initial data
                var initialData = new[] { new Species("Ant"), new Species("Ape"), new Species("Bear"), new Species("Boar"), new Species("Cougar")};
                sourceList.AddRange(initialData);
                
                //Check all data has loaded
                grouper.SpeciesByLetter.Items.SelectMany(g => g.Items).ShouldAllBeEquivalentTo(initialData);
                grouper.SpeciesByLetter.Items.ShouldBeEquivalentTo(manualGrouping(sourceList));
                grouper.SpeciesByLetter.Count.Should().Be(3);

                //change the first letter of the data and the groupings will change
                initialData[0].Name = "Ánt"; //change the first letter
                grouper.SpeciesByLetter.Count.Should().Be(4);

                //assert everything
                grouper.SpeciesByLetter.Items.ShouldBeEquivalentTo(manualGrouping(sourceList));
            }
        }
    }
}