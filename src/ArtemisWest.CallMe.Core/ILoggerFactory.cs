namespace ArtemisWest.CallMe
{
    //Check if I can do some thing smarter with a Func<ILoggerFacade>
    public interface ILoggerFactory
    {
        ILogger GetLogger();
    }
}