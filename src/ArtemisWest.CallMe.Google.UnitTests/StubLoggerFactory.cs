namespace ArtemisWest.CallMe.Google.UnitTests
{
    public class StubLoggerFactory : ILoggerFactory
    {
        public ILogger GetLogger()
        {
         return new NullLogger();   
        }
    }
}