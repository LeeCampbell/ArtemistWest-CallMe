﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;

namespace ArtemisWest.CallMe.Google
{
    public sealed class HttpRequestParameters
    {
        private readonly string _endPointUrl;
        private readonly NameValueCollection _queryStringParameters = new NameValueCollection();
        private readonly NameValueCollection _postParameters = new NameValueCollection();
        private readonly NameValueCollection _headers = new NameValueCollection();

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

            if (PostParameters.Count > 0)
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                var postArguments = System.Web.HttpUtility.ParseQueryString(string.Empty);
                foreach (var key in PostParameters.AllKeys)
                {
                    postArguments[key] = PostParameters[key];
                }
                using (var requestStream = request.GetRequestStream())
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(postArguments.ToString());
                }
            }

            return request;
        }

        public Uri ConstructUri()
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
            var headers = ToString(Headers);
            var post = ToString(PostParameters);

            return string.Format("{0} HEADERS:{1} POST:{2}", ConstructUri(), headers, post);
        }

        private static string ToString(NameValueCollection nvc)
        {
            return string.Join(", ", nvc.AllKeys.Select(k => string.Format("[{0}:{1}]", k, nvc[k])));
        }
    }
}
