using System.Reactive.Concurrency;

namespace ArtemisWest.CallMe
{
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler CurrentThread
        {
            get { return Scheduler.CurrentThread; }
        }

        public IDispatcherScheduler Dispatcher
        {
            get { return new DispatcherScheduler(); }
        }
        
        public IScheduler Immediate
        {
            get { return Scheduler.Immediate; }
        }

        public IScheduler NewThread
        {
            get { return NewThreadScheduler.Default; }
        }

        public IScheduler ThreadPool
        {
            get { return ThreadPoolScheduler.Instance; }
        }
    }

}
