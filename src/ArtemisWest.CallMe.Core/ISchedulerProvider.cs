using System.Reactive.Concurrency;

namespace ArtemisWest.CallMe
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }   //Onl really server side or Console, else use Recursue scheduling.

        IDispatcherScheduler Dispatcher { get; }

        IScheduler Immediate { get; }   //When would I ever need this? This is only for testing yeah?

        IScheduler NewThread { get; }   //Only Long running stuff

        IScheduler ThreadPool { get; }  //Default for Concurrency

        //Immediate/CurrentThread 
        //Async                     -Dispatcher
        //Concurrent                -ThreadPool/TaskPool
        //LongRunning               -LongRunningTaskpool or NewThread
    }
}