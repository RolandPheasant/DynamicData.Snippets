using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicData.Snippets.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Watch
{
    public class SelectCacheItemFixture
    {

        [Fact]
        public void SelectedItemsTests()
        {
            var schedulerProvider = new TestSchedulerProvider();
            var items = Enumerable.Range(1, 10).Select(i => new CacheItem(i.ToString())).ToArray();
            
            using (var sut = new SelectCacheItem(schedulerProvider))
            {
                sut.Load(items);

                sut.SelectedItem.Id.Should().Be("1");

                sut.AddOrUpdate(new CacheItem("11"));

                sut.SelectedItem.Id.Should().Be("11");
            }

        }
    }
}
