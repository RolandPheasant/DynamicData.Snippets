using System;

namespace DynamicData.Snippets.ViewModelTesting
{
    public interface IDataProvider
    {
        IObservableCache<Item, int> ItemCache { get; }
    }

    public class Item
    {
        public int Id { get; }

        public Item(int id)
        {
            Id = id;
        }
    }

    public class ItemViewModel
    {
        public Item Item { get; }

        public ItemViewModel(Item item)
        {
            Item = item;
        }
    }
}