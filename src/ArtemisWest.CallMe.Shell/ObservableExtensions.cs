using System;
using System.Reactive.Linq;
using Microsoft.Practices.Prism.Logging;
using log4net.Core;

namespace ArtemisWest.CallMe.Shell
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Log<T>(this IObservable<T> source, ILoggerFacade logger, string name)
        {
            return source.Do
                (
                //HACK: get to fixing this to actually use the logger!! -LC

                    i => Console.WriteLine("{0}.OnNext({1})", name, i),
                    ex => Console.WriteLine("{0}.OnError({1})", name, ex),
                    () => Console.WriteLine("{0}.OnComplete()", name)
                );
        }
    }
}