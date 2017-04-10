namespace DynamicData.Snippets.Infrastructure
{
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