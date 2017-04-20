using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit;
using NUnit.Framework;

namespace DynamicData.Snippets.Switch
{
    [TestFixture]
    public class SwitchDataSourceFixture
    {
        [Test]
        public void Switch()
        {
            using (var listA = new SourceList<int>())
            using (var listB = new SourceList<int>())
            using (var sut = new SwitchDataSource(listA, listB))
            {
                var oddNumbers = Enumerable.Range(1, 1000).Where(i => i % 2 == 1).ToArray();
                var evenNumbers = Enumerable.Range(1, 1000).Where(i => i % 2 == 2).ToArray();

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
