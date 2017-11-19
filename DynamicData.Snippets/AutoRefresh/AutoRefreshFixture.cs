using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.AutoRefresh
{
    public class AutoRefreshFixture
    {
        [Fact]
        public void AutoRefresh()
        {
            var items = new List<MutableThing>
            {
                new MutableThing(1, "A"),
                new MutableThing(2, "A"),
                new MutableThing(3, "B"),
                new MutableThing(4, "C"),
                new MutableThing(5, "D"),
                new MutableThing(6, "D"),

            };
            //result should only be true when all items are set to true
            using (var cache = new SourceCache<MutableThing, int>(m => m.Id))
            {
                var sut = new AutoRefreshForPropertyChanges(cache);

                int count = 0;
                sut.DistinctCount.Subscribe(result => count = result);

                cache.AddOrUpdate(items);
                count.Should().Be(4);

                //check mutating a value works
                items[2].Value = "A";
                count.Should().Be(3);

                //check remove works
                cache.RemoveKey(4);
                count.Should().Be(2);

                //check add works
                cache.AddOrUpdate(new MutableThing(10, "z"));
                count.Should().Be(3);

            }
        }
    }
}
