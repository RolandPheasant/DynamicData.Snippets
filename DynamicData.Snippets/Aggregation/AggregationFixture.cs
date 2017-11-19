using System.Linq;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Aggregation
{
    public class AggregationFixture
    {
        [Fact]
        public void Aggregations()
        {
            using (var dataSource = new SourceList<int>())
            using (var sut = new Aggregations(dataSource))
            {
                //check StartWithEmpty() has taken effect 
                sut.Min.Should().NotBeNull();
                sut.Max.Should().NotBeNull();
                sut.Avg.Should().NotBeNull();
                
                dataSource.AddRange(Enumerable.Range(1, 10));

                sut.Min.Should().Be(1);
                sut.Max.Should().Be(10);
                sut.Avg.Should().Be(5.5);

                dataSource.RemoveRange(0,9);
                dataSource.Add(100);

                //items in list = [10,100]
                sut.Min.Should().Be(10);
                sut.Max.Should().Be(100);
                sut.Avg.Should().Be(55);
            }
        }
    }
}