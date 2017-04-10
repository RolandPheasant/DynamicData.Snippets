using System;
using System.Reactive.Subjects;

namespace DynamicData.Snippets.Infrastructure
{
    public class SimpleObjectWithObservable 
    {
        public int Id { get; }

        private readonly ISubject<bool> _isActiveSubject = new Subject<bool>();

        public SimpleObjectWithObservable(int id)
        {
            Id = id;
        }

        public IObservable<bool> IsActive => _isActiveSubject;

        public void SetIsActive(bool isActive)
        {
            _isActiveSubject.OnNext(isActive);
        }
    }
}