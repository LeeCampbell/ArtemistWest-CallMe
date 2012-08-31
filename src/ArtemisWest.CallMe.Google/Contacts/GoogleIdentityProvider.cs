using System;
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

        public GoogleIdentityProvider(Authorization.IAuthorizationModel authorizationModel, IWebRequstService webRequstService)
        {
            _authorizationModel = authorizationModel;
            _webRequstService = webRequstService;
        }

        public IObservable<IProfile> FindProfile(string query)
        {
            return (
                    from request in _authorizationModel.RequestAccessToken().Select(token=>CreateRequestParams(query,token))
                            .Log("IdentityRequestParams")
                    from response in _webRequstService.GetResponse(request)
                            .Log("IdentityResponse")
                    select Translate(response)
                    )
                    .Take(1);
        }

        private WebRequestParameters CreateRequestParams(string query, string accessToken)
        {
            var param = new WebRequestParameters(@"https://www.google.com/m8/feeds/contacts/default/full");
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

            var emails = from email in xDoc.Root.Element(entryXName).XPathSelectElements("gd:email", ns)
                         select new PersonalIdentifier(GoogleProviderDescription.Instance, "email", email.Attribute("address").Value);
            var phonenumbers = from phone in xDoc.Root.Element(entryXName).XPathSelectElements("gd:phoneNumber", ns)
                               select new PersonalIdentifier(GoogleProviderDescription.Instance, "phone", phone.Value);

            return new Profile(emails.Concat(phonenumbers));
        }
    }
}