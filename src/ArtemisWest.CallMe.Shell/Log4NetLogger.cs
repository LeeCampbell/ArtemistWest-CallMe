using System;
using Microsoft.Practices.Prism.Logging;
using log4net;
using log4net.Core;

namespace ArtemisWest.CallMe.Shell
{
    public sealed class Log4NetLogger : ILogger, ILoggerFacade
    {
        private readonly ILog _log;

        public Log4NetLogger(Type callingType)
        {
            _log = LogManager.GetLogger(callingType);
        }

        public void Write(LogLevel level, string message, Exception exception)
        {
            _log.Logger.Log(null, ToLog4Net(level), message, exception);
        }

        public IDisposable Scope(string name)
        {
            //TODO: Add a GUID correlation/context id?
            //return log4net.GlobalContext.Stacks["NDC"].Push(name);
            //return log4net.ThreadContext.Stacks["NDC"].Push(name);
            return log4net.LogicalThreadContext.Stacks["NDC"].Push(name);
            //return NDC.Push(name);
        }

        public void Log(string message, Category category, Priority _)
        {
            _log.Logger.Log(null, ToLog4Net(category), message, null);
        }

        private static Level ToLog4Net(Category category)
        {
            switch (category)
            {
                case Category.Debug:
                    return Level.Debug;
                case Category.Exception:
                    return Level.Error;
                case Category.Info:
                    return Level.Info;
                case Category.Warn:
                    return Level.Warn;
                default:
                    throw new ArgumentOutOfRangeException("category");
            }
        }

        private static Level ToLog4Net(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return Level.Verbose;
                case LogLevel.Trace:
                    return Level.Trace;
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Fatal:
                    return Level.Fatal;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }
    }
}
