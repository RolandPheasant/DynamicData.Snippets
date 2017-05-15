using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicData.Snippets.MutableValues
{
    [TestFixture]
    public class AutoRefreshFixture
    {
        public enum ForceEvaluationMode
        {
            Cache,
            Observable
        }

        [TestCase(ForceEvaluationMode.Cache)]
        [TestCase(ForceEvaluationMode.Observable)]
        public void AutoRefresh(ForceEvaluationMode mode)
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
                var sut = mode == ForceEvaluationMode.Cache
                    ? new AutoRefreshForPropertyChanges(cache)
                    : new AutoRefreshForPropertyChanges(cache.Connect());

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
