using DynamicData.Binding;

namespace DynamicData.Snippets.Infrastructure
{
    public class SimpleNotifyPropertyChangedObject: AbstractNotifyPropertyChanged
    {
        public int Id { get; }

        public SimpleNotifyPropertyChangedObject(int id)
        {
            Id = id;
        }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => SetAndRaise(ref _isActive, value);
        }
    }
}