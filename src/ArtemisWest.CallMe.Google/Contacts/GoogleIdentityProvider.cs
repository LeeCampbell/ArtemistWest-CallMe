using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Google.Authorization;

namespace ArtemisWest.CallMe.Google.Contacts
{
    public class GoogleIdentityProvider : IIdentityProvider
    {
        private readonly IAuthorizationModel _authorizationModel;
        private readonly IWebRequstService _webRequstService;
        private readonly ILogger _logger;

        public GoogleIdentityProvider(Authorization.IAuthorizationModel authorizationModel, IWebRequstService webRequstService, ILoggerFactory loggerFactory)
        {
            _authorizationModel = authorizationModel;
            _webRequstService = webRequstService;
            _logger = loggerFactory.GetLogger();
        }
        public IObservable<IProfile> FindProfile(IList<string> identityKeys)
        {
            return (
                    from request in _authorizationModel.RequestAccessToken()
                            .Select(token => CreateRequestParams(identityKeys, token))
                            .Log(_logger, "IdentityRequestParams")
                    from response in _webRequstService.GetResponse(request)
                            .Log(_logger, "IdentityResponse")
                    select Translate(response)
                    )
                    .Take(1);
        }

        private HttpRequestParameters CreateRequestParams(IList<string> identityKeys, string accessToken)
        {
            //var query = string.Join(" ", identityKeys);
            //HACK: Need to be able to query Google contacts for multiple keys.
            var query = identityKeys.First();
            var param = new HttpRequestParameters(@"https://www.google.com/m8/feeds/contacts/default/full");
            param.QueryStringParameters.Add("access_token", accessToken);
            param.QueryStringParameters.Add("q", query);
            param.Headers.Add("GData-Version", "3.0");
            return param;
        }

        private static IProfile Translate(string response)
        {
            var xDoc = XDocument.Parse(response);

            var ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace("x", "http://www.w3.org/2005/Atom");
            ns.AddNamespace("openSearch", "http://a9.com/-/spec/opensearch/1.1/");
            ns.AddNamespace("gContact", "http://schemas.google.com/contact/2008");
            ns.AddNamespace("batch", "http://schemas.google.com/gdata/batch");
            ns.AddNamespace("gd", "http://schemas.google.com/g/2005");

            var entryXName = XName.Get("entry", "http://www.w3.org/2005/Atom");

            //TODO: Handle an empty result set - LC
            var emails = from email in xDoc.Root.Element(entryXName).XPathSelectElements("gd:email", ns)
                         select new PersonalIdentifier(GoogleProviderDescription.Instance, "email", email.Attribute("address").Value);
            var phonenumbers = from phone in xDoc.Root.Element(entryXName).XPathSelectElements("gd:phoneNumber", ns)
                               select new PersonalIdentifier(GoogleProviderDescription.Instance, "phone", phone.Value);

            return new Profile(emails.Concat(phonenumbers));
        }

        
    }
    public class OfflineIdentityProvider : IIdentityProvider
    {
        private readonly IProviderDescription _providerDescription;

        public OfflineIdentityProvider(IProviderDescription providerDescription)
        {
            _providerDescription = providerDescription;
        }

        public IObservable<IProfile> FindProfile(IList<string> _)
        {
            return Observable.Return(new Profile(new IPersonalIdentifier[]
                                                     {
                                                         new PersonalIdentifier(_providerDescription, "email", "lee@home.com"),
                                                         new PersonalIdentifier(_providerDescription, "email", "lee@work.com"),
                                                         new PersonalIdentifier(_providerDescription, "phone", "+64212543824"),
                                                         new PersonalIdentifier(_providerDescription, "email", "@LeeCampbell"),
                                                     }));
        }
    }
}