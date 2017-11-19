using System;
using System.Linq;
using System.Reactive.Linq;

namespace DynamicData.Snippets.InspectItems
{
    public class InspectCollection : IDisposable
    {
        private readonly IDisposable _cleanUp;

        public InspectCollection(ISourceList<SimpleImmutableObject> source)
        {
            /*
                Example to illustrate how to inspect an entire collection when items are added or removed
            */

            _cleanUp = source.Connect()
                .ToCollection()
                .Select(items =>
                    {
                        return new
                        {
                            DistinctCount = items.Select(x => x.Value).Distinct().Count(),
                            Count = items.Count
                        };
                    }
                )
                .Subscribe(x =>
                {
                    DistinctCount = x.DistinctCount;
                    Count = x.Count;
                });
        }

        public int DistinctCount { get; set; }
        public int Count { get; set; }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class SimpleImmutableObject
    {
        public int Id { get; }
        public string Value { get; }

        public SimpleImmutableObject(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}
