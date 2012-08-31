using System;
using System.IO;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ArtemisWest.CallMe.Google
{
    public class WebRequstService : IWebRequstService
    {
        public IObservable<string> GetResponse(WebRequestParameters requestParameters)
        {
            return Observable.Create<string>(
                o =>
                    {
                        var request = CreateRequest(requestParameters);

                        var response = request.GetResponse();
                        var responseStream = response.GetResponseStream();
                        using (var reader = new StreamReader(responseStream))
                        {
                            string payload = reader.ReadToEnd();
                            o.OnNext(payload);
                            o.OnCompleted();
                            return Disposable.Empty;
                        }
                        //return Task.Factory
                        //    .FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null)
                        //    .ToObservable()
                        //    .Select(response =>
                        //                {
                        //                    Console.WriteLine("Select(response =>");
                        //                    var responseStream = response.GetResponseStream();
                        //                    using (var reader = new StreamReader(responseStream))
                        //                    {
                        //                        return reader.ReadToEnd();
                        //                    }
                        //                })
                        //    .Subscribe(o);
                    });

        }

        private HttpWebRequest CreateRequest(WebRequestParameters requestParameters)
        {
            var queryUri = new UriBuilder(requestParameters.EndPointUrl);

            if (requestParameters.QueryStringParameters.Count > 0)
            {
                var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var key in requestParameters.QueryStringParameters.AllKeys)
                {
                    queryString[key] = requestParameters.QueryStringParameters[key];
                }
                queryUri.Query = queryString.ToString();// Returns "key1=value1&key2=value2", all URL-encoded
            }
            
            var request = (HttpWebRequest)WebRequest.Create(queryUri.Uri);

            foreach (var key in requestParameters.Headers.AllKeys)
            {
                request.Headers.Add(key, requestParameters.Headers[key]);
            }

            return request;
        }
    }
}