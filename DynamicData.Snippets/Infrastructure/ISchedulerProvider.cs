using System.Reactive.Concurrency;

namespace DynamicData.Snippets.Infrastructure
{
    public interface ISchedulerProvider
    {
        IScheduler MainThread { get; }
        IScheduler Background { get; }
    }
}
