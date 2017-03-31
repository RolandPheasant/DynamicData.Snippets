using System.Reactive.Concurrency;
using System.Windows.Threading;
using Microsoft.Reactive.Testing;

namespace DynamicData.Snippets.Infrastructure
{
    public interface ISchedulerProvider
    {
        IScheduler MainThread { get; }
        IScheduler Background { get; }
    }

    public class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainThread { get; }

        public IScheduler Background { get; } = TaskPoolScheduler.Default;

        public SchedulerProvider(Dispatcher dispatcher)
        {
            MainThread = new DispatcherScheduler(dispatcher);
        }

    }

    public class TestSchedulerProvider : ISchedulerProvider
    {
        public TestScheduler TestScheduler { get; } = new TestScheduler();

        private readonly IScheduler _mainThread;
        private readonly IScheduler _background;
        IScheduler ISchedulerProvider.MainThread => _mainThread;
        IScheduler ISchedulerProvider.Background => _background;

        public TestSchedulerProvider()
        {
            _mainThread = Scheduler.Immediate;
            _background = TestScheduler;
        }
    }
}
