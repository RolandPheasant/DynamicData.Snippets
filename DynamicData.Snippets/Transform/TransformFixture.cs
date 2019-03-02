using System.Linq;
using DynamicData.Binding;
using FluentAssertions;
using Xunit;

namespace DynamicData.Snippets.Transform
{
    
    public class TransformFixture
    {
        [Fact]
        public void FlattenObservableCollection()
        {
            var children = new[]
            {
                new NestedChild("A", "ValueA"),
                new NestedChild("B", "ValueB"),
                new NestedChild("C", "ValueC"),
                new NestedChild("D", "ValueD"),
                new NestedChild("E", "ValueE"),
                new NestedChild("F", "ValueF")
            };

            var parents = new[]
            {
                new ClassWithNestedObservableCollection(1, new[] { children[0], children[1] }),
                new ClassWithNestedObservableCollection(2, new[] { children[2], children[3] }),
                new ClassWithNestedObservableCollection(3, new[] { children[4] })
            };

            using (var source = new SourceCache<ClassWithNestedObservableCollection, int>(x => x.Id))
            using (var sut = new FlattenNestedObservableCollection(source))
            {
                source.AddOrUpdate(parents);

                sut.Children.Count.Should().Be(5);
                sut.Children.Items.ShouldBeEquivalentTo(parents.SelectMany(p => p.Children.Take(5)));

                //add a child to the observable collection
                parents[2].Children.Add(children[5]);

                sut.Children.Count.Should().Be(6);
                sut.Children.Items.ShouldBeEquivalentTo(parents.SelectMany(p => p.Children));

                //remove a parent and check children have moved
                source.RemoveKey(1);
                sut.Children.Count.Should().Be(4);
                sut.Children.Items.ShouldBeEquivalentTo(parents.Skip(1).SelectMany(p => p.Children));

                //add a parent and check items have been added back in
                source.AddOrUpdate(parents[0]);

                sut.Children.Count.Should().Be(6);
                sut.Children.Items.ShouldBeEquivalentTo(parents.SelectMany(p => p.Children));
            }
        }

        [Fact]
        public void FlattenObservableCollectionWithProjection()
        {
            var children = new[]
            {
                new NestedChild("A", "ValueA"),
                new NestedChild("B", "ValueB"),
                new NestedChild("C", "ValueC"),
                new NestedChild("D", "ValueD"),
                new NestedChild("E", "ValueE"),
                new NestedChild("F", "ValueF")
            };

            var parents = new[]
            {
                new ClassWithNestedObservableCollection(1, new[] { children[0], children[1] }),
                new ClassWithNestedObservableCollection(2, new[] { children[2], children[3] }),
                new ClassWithNestedObservableCollection(3, new[] { children[4] })
            };

            using (var source = new SourceCache<ClassWithNestedObservableCollection, int>(x => x.Id))
            using (var sut = source.Connect()
                        .AutoRefreshOnObservable(self => self.Children.ToObservableChangeSet())
                        .TransformMany(parent => parent.Children.Select(c => new ProjectedNestedChild(parent, c)), c => c.Child.Name)
                        .AsObservableCache())
            {
                source.AddOrUpdate(parents);

                sut.Count.Should().Be(5);
                sut.Items.ShouldBeEquivalentTo(parents.SelectMany(p => p.Children.Take(5).Select(c => new ProjectedNestedChild(p, c))));

                //add a child to the observable collection
                parents[2].Children.Add(children[5]);

                sut.Count.Should().Be(6);
                sut.Items.ShouldBeEquivalentTo(parents.SelectMany(p => p.Children.Select(c => new ProjectedNestedChild(p, c))));

                //remove a parent and check children have moved
                source.RemoveKey(1);
                sut.Count.Should().Be(4);
                sut.Items.ShouldBeEquivalentTo(parents.Skip(1).SelectMany(p => p.Children.Select(c => new ProjectedNestedChild(p, c))));

                //add a parent and check items have been added back in
                source.AddOrUpdate(parents[0]);

                sut.Count.Should().Be(6);
                sut.Items.ShouldBeEquivalentTo(parents.SelectMany(p => p.Children.Select(c => new ProjectedNestedChild(p, c))));
            }
        }

        private class ProjectedNestedChild
        {
            public ClassWithNestedObservableCollection Parent { get; }

            public NestedChild Child { get; }

            public ProjectedNestedChild(ClassWithNestedObservableCollection parent, NestedChild child)
            {
                Parent = parent;
                Child = child;
            }
        }
    }
}