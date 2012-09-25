using System;
using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace ArtemisWest.CallMe
{
    public sealed class DispatcherScheduler : IDispatcherScheduler
    {
        private readonly Dispatcher _currentDispatcher;
        private readonly System.Reactive.Concurrency.DispatcherScheduler _underlying;

        public DispatcherScheduler()
        {
            _currentDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            _underlying = new System.Reactive.Concurrency.DispatcherScheduler(_currentDispatcher);
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            return _underlying.Schedule(state, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return _underlying.Schedule(state, dueTime, action);
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            return _underlying.Schedule(state, dueTime, action);
        }

        public DateTimeOffset Now
        {
            get { return _underlying.Now; }
        }

        public void Schedule(Action action, DispatcherPriority dispatcherPriority)
        {
            _currentDispatcher.BeginInvoke(action, dispatcherPriority);
        }

        public void Invoke(Action action)
        {
            if (_currentDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Schedule(action, DispatcherPriority.Normal);
            }
        }
    }
}