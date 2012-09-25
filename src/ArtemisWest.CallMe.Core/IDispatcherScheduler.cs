using System;
using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace ArtemisWest.CallMe
{
    public interface IDispatcherScheduler : IScheduler
    {
        void Schedule(Action action, DispatcherPriority dispatcherPriority);
        void Invoke(Action action);
    }
}