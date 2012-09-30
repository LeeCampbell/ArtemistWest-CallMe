using System.Diagnostics;

namespace ArtemisWest.CallMe.Shell
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger GetLogger()
        {
            var callersStackFrame = new StackFrame(1);
            var callerMethod = callersStackFrame.GetMethod();
            var callingType = callerMethod.ReflectedType;
            return new Log4NetLogger(callingType);
        }
    }
}