using System;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Contract.Contacts;
using ArtemisWest.CallMe.Google.Authorization;

namespace ArtemisWest.CallMe.Google.Contacts
{
    public sealed class GoogleContactQueryProvider : IContactQueryProvider
    {
        private readonly IAuthorizationModel _authorizationModel;
        private readonly IWebRequstService _webRequstService;

        public GoogleContactQueryProvider(Authorization.IAuthorizationModel authorizationModel, IWebRequstService webRequstService)
        {
            _authorizationModel = authorizationModel;
            _webRequstService = webRequstService;
        }

        public IObservable<string> Search(IProfile activeProfile)
        {
            Console.WriteLine("GQCP.Search({0})", activeProfile);
            return (
                       from request in _authorizationModel.RequestAccessToken()
                            .Log("GQCPRequestAccessToken")
                           .Select(token => CreateRequestParams(activeProfile, token))
                            .Log("ContactRequestParams")
                       from response in _webRequstService.GetResponse(request)
                            .Log("ContactResponse")
                       select Translate(response).ToString()
                   )
                .Take(1);
        }

        private HttpRequestParameters CreateRequestParams(IProfile activeProfile, string accessToken)
        {
            //var query = string.Join(" ", activeProfile.Identifiers.Select(i => i.Value));
            var query = activeProfile.Identifiers.Select(i => i.Value).FirstOrDefault() ?? string.Empty;

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

            var emails = from email in xDoc.Root.Element(entryXName).XPathSelectElements("gd:email", ns)
                         select new PersonalIdentifier(GoogleProviderDescription.Instance, "email", email.Attribute("address").Value);
            var phonenumbers = from phone in xDoc.Root.Element(entryXName).XPathSelectElements("gd:phoneNumber", ns)
                               select new PersonalIdentifier(GoogleProviderDescription.Instance, "phone", phone.Value);

            return new Profile(emails.Concat(phonenumbers));
        }
    }
}