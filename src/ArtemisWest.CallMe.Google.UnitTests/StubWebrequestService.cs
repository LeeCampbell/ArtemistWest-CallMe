using System;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe.Google.UnitTests
{
    public class StubWebrequestService : IWebRequstService
    {
        public string Response { get; set; }
        public IObservable<string> GetResponse(HttpRequestParameters requestParameters)
        {
            return Observable.Return(Response);
        }
    }
}