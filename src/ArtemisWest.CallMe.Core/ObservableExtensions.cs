using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Log<T>(this IObservable<T> source, ILogger logger, string name)
        {
            return source.Do
                (                
                    i => logger.Trace("{0}.OnNext({1})", name, i),
                    ex => logger.Trace("{0}.OnError({1})", name, ex),
                    () => logger.Trace("{0}.OnComplete()", name)
                );
        }

        //TODO: Could potentially upgrade to using tasks/Await-LC
        public static IObservable<byte> ToObservable(
            this Stream source,
            int buffersize,
            IScheduler scheduler)
        {
            var bytes = Observable.Create<byte>(o =>
            {
                var initialState = new StreamReaderState(source, buffersize);
                var currentStateSubscription = new SerialDisposable();
                Action<StreamReaderState, Action<StreamReaderState>> iterator =
                (state, self) =>
                    currentStateSubscription.Disposable = state.ReadNext()
                        .Subscribe(
                            bytesRead =>
                            {
                                for (int i = 0; i < bytesRead; i++)
                                {
                                    o.OnNext(state.Buffer[i]);
                                }
                                if (bytesRead > 0)
                                    self(state);
                                else
                                    o.OnCompleted();
                            },
                            o.OnError);
                var scheduledWork = scheduler.Schedule(initialState, iterator);
                return new CompositeDisposable(currentStateSubscription, scheduledWork);
            });
            return Observable.Using(() => source, _ => bytes);
        }

        private sealed class StreamReaderState
        {
            private readonly int _bufferSize;
            private readonly Func<byte[], int, int, IObservable<int>> _factory;
            public StreamReaderState(Stream source, int bufferSize)
            {
                _bufferSize = bufferSize;
                _factory = Observable.FromAsyncPattern<byte[], int, int, int>(
                source.BeginRead,
                source.EndRead);
                Buffer = new byte[bufferSize];
            }
            public IObservable<int> ReadNext()
            {
                return _factory(Buffer, 0, _bufferSize);
            }
            public byte[] Buffer { get; set; }
        }
    }
}
