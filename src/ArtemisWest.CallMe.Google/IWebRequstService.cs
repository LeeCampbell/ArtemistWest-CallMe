using System;

namespace ArtemisWest.CallMe.Google
{
    public interface IWebRequstService
    {
        IObservable<string> GetResponse(HttpRequestParameters requestParameters);
    }
}