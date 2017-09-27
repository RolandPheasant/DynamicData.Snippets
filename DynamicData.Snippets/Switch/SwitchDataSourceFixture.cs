using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Switch
{
    
    public class SwitchDataSourceFixture
    {
        [Fact]
        public void Switch()
        {
            using (var listA = new SourceList<int>())
            using (var listB = new SourceList<int>())
            using (var sut = new SwitchDataSource(listA, listB))
            {
                var oddNumbers = Enumerable.Range(1, 10000).Where(i => i % 2 == 1).ToArray();
                var evenNumbers = Enumerable.Range(1, 10000).Where(i => i % 2 == 2).ToArray();

                listA.AddRange(oddNumbers);
                listB.AddRange(evenNumbers);

                sut.DataSource.Items.ShouldAllBeEquivalentTo(oddNumbers);

                sut.Option = SwitchDataSourceOption.SourceB;
                sut.DataSource.Items.ShouldAllBeEquivalentTo(evenNumbers);

                sut.Option = SwitchDataSourceOption.SourceA;
                sut.DataSource.Items.ShouldAllBeEquivalentTo(oddNumbers);
            }
        }
    }
}
