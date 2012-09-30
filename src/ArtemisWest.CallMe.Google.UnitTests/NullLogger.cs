using System;
using System.Reactive.Disposables;

namespace ArtemisWest.CallMe.Google.UnitTests
{
    public class NullLogger : ILogger 
    {
        public void Write(LogLevel level, string message, Exception exception)
        {
        }

        public IDisposable Scope(string name)
        {
            return Disposable.Empty;
        }
    }
}