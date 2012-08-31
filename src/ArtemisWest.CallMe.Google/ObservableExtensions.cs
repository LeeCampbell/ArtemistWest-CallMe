using System;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe.Google
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Log<T>(this IObservable<T> source, string name)
        {
            return source.Do
                (
                    i => Console.WriteLine("{0}.OnNext({1})", name, i),
                    ex => Console.WriteLine("{0}.OnError({1})", name, ex),
                    () => Console.WriteLine("{0}.OnComplete()", name)
                );
        }
    }
}