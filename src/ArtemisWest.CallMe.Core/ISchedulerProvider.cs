using System.Reactive.Concurrency;

namespace ArtemisWest.CallMe
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }

        IDispatcherScheduler Dispatcher { get; }

        IScheduler Immediate { get; }

        IScheduler NewThread { get; }

        IScheduler ThreadPool { get; }
    }
}