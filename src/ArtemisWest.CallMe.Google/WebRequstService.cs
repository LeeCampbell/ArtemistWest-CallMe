using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe.Google
{
    public class WebRequstService : IWebRequstService
    {
        private readonly ILogger _logger;

        public WebRequstService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.GetLogger();
        }

        public IObservable<string> GetResponse(HttpRequestParameters requestParameters)
        {
            return Observable.Create<string>(
                o =>
                    {
                        _logger.Debug("GetResponse({0})", requestParameters);

                        var request = requestParameters.CreateRequest();
                        
                        var response = request.GetResponse();
                        var responseStream = response.GetResponseStream();
                        if(responseStream==null)
                        {
                            o.OnCompleted();
                            return Disposable.Empty;
                        }
                        using (var reader = new StreamReader(responseStream))
                        {
                            string payload = reader.ReadToEnd();
                            _logger.Debug("GetResponse({0}) returned {1}", requestParameters, payload);
                            o.OnNext(payload);
                            o.OnCompleted();
                            return Disposable.Empty;
                        }
                        //return Task.Factory
                        //    .FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null)
                        //    .ToObservable()
                        //    .Select(response =>
                        //                {
                        //                    var responseStream = response.GetResponseStream();
                        //                    using (var reader = new StreamReader(responseStream))
                        //                    {
                        //                        return reader.ReadToEnd();
                        //                    }
                        //                })
                        //    .Subscribe(o);
                    });

        }
    }
}