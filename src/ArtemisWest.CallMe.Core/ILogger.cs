using System;

namespace ArtemisWest.CallMe
{
    //TODO: Document interface. -LC
    public interface ILogger
    {
        void Write(LogLevel level, string message, Exception exception);
        IDisposable Scope(string name);   
    }
}