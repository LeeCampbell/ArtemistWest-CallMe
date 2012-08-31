using System.Collections.Specialized;

namespace ArtemisWest.CallMe.Google
{
    public sealed class WebRequestParameters
    {
        private readonly string _endPointUrl;
        private readonly NameValueCollection _queryStringParameters = new NameValueCollection(); 
        private readonly NameValueCollection _postParameters = new NameValueCollection(); 
        private readonly NameValueCollection _headers= new NameValueCollection(); 

        public WebRequestParameters(string endPointUrl)
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
    }
}
