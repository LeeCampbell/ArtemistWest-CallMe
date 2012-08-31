using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace ArtemisWest.CallMe.Google
{
    public sealed class HttpRequestParameters
    {
        private readonly string _endPointUrl;
        private readonly NameValueCollection _queryStringParameters = new NameValueCollection(); 
        private readonly NameValueCollection _postParameters = new NameValueCollection(); 
        private readonly NameValueCollection _headers= new NameValueCollection(); 

        public HttpRequestParameters(string endPointUrl)
        {
            _endPointUrl = endPointUrl;
        }

        public string EndPointUrl
        {
            get { return _endPointUrl; }
        }

        public NameValueCollection QueryStringParameters
        {
            get { return _queryStringParameters; }
        }

        public NameValueCollection PostParameters
        {
            get { return _postParameters; }
        }

        public NameValueCollection Headers
        {
            get { return _headers; }
        }


        public HttpWebRequest CreateRequest()
        {
            var queryUri = ConstructUri();

            var request = (HttpWebRequest)WebRequest.Create(queryUri);

            foreach (var key in Headers.AllKeys)
            {
                request.Headers.Add(key, Headers[key]);
            }

            return request;
        }

        private Uri ConstructUri()
        {
            var uriBuilder = new UriBuilder(EndPointUrl);

            if (QueryStringParameters.Count > 0)
            {
                var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var key in QueryStringParameters.AllKeys)
                {
                    queryString[key] = QueryStringParameters[key];
                }
                uriBuilder.Query = queryString.ToString(); // Returns "key1=value1&key2=value2", all URL-encoded
            }
            return uriBuilder.Uri;
        }

        public override string ToString()
        {
            var headers = string.Join(
                ", ",
                Headers.AllKeys.Select(k => string.Format("[{0}:{1}]", k, Headers[k])));

            return string.Format("{0} HEADERS:{1}", ConstructUri(), headers);
        }

    }
}
