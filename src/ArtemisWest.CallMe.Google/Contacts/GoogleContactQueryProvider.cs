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
        private static readonly XmlNamespaceManager Ns;

        static GoogleContactQueryProvider()
        {
            Ns = new XmlNamespaceManager(new NameTable());
            Ns.AddNamespace("x", "http://www.w3.org/2005/Atom");
            Ns.AddNamespace("openSearch", "http://a9.com/-/spec/opensearch/1.1/");
            Ns.AddNamespace("gContact", "http://schemas.google.com/contact/2008");
            Ns.AddNamespace("batch", "http://schemas.google.com/gdata/batch");
            Ns.AddNamespace("gd", "http://schemas.google.com/g/2005");
        }

        public GoogleContactQueryProvider(IAuthorizationModel authorizationModel, IWebRequstService webRequstService)
        {
            _authorizationModel = authorizationModel;
            _webRequstService = webRequstService;
        }

        public IObservable<IContact> Search(IProfile activeProfile)
        {
            Console.WriteLine("GCQP.Search({0})", activeProfile);
            return (
                       from accessToken in _authorizationModel.RequestAccessToken()
                           .Log("GCQPRequestAccessToken")
                       from request in Observable.Return(CreateRequestParams(activeProfile, accessToken))
                           //.Select(token => CreateRequestParams(activeProfile, token))
                           .Log("ContactRequestParams")
                       from response in _webRequstService.GetResponse(request)
                       //.Log("ContactResponse")
                       select Translate(response, accessToken)
                   )
                .Take(1);
        }

        private HttpRequestParameters CreateRequestParams(IProfile activeProfile, string accessToken)
        {
            //var query = string.Join(" ", activeProfile.Identifiers.Select(i => i.Value));
            //HACK: Only searching on single Identifier at the moment.
            var query = activeProfile.Identifiers.Select(i => i.Value).FirstOrDefault() ?? string.Empty;

            var param = new HttpRequestParameters(@"https://www.google.com/m8/feeds/contacts/default/full");
            param.QueryStringParameters.Add("access_token", accessToken);
            param.QueryStringParameters.Add("q", query);
            param.Headers.Add("GData-Version", "3.0");
            return param;
        }

        private static IContact Translate(string response, string accessToken)
        {
            var xDoc = XDocument.Parse(response);
            var entryXName = XName.Get("entry", "http://www.w3.org/2005/Atom");

            var xContactEntry = xDoc.Root.Element(entryXName);
            if (xContactEntry == null)
                throw new InvalidOperationException(); //TODO: Support not finding anything.

            var result = new Contact();
            result.Title = XPathString(xContactEntry, "x:title", Ns);
            result.FullName = XPathString(xContactEntry, "gd:name/gd:fullName", Ns);
            result.DateOfBirth = xContactEntry.Elements(ToXName("gContact", "birthday"))
                                              .Select(x => (DateTime?)x.Attribute("when"))
                                              .FirstOrDefault();
            result.Image = xContactEntry.Elements(ToXName("x", "link"))
                .Where(x => x.Attribute("rel") != null
                            && x.Attribute("rel").Value == "http://schemas.google.com/contacts/2008/rel#photo"
                            && x.Attribute("type") != null
                            && x.Attribute("type").Value == "image/*"
                            && x.Attribute("href") != null)
                .Select(x => x.Attribute("href"))
                .Where(att => att != null)
                //TODO: Add param.QueryStringParameters.Add("access_token", accessToken); to the URI
                //.Select(att => new Uri(att.Value))
                .Select(att =>
                            {
                                var hrp = new HttpRequestParameters(att.Value);
                                hrp.QueryStringParameters.Add("access_token", accessToken);
                                return hrp.ConstructUri();
                            })
                .FirstOrDefault();

            //<gd:email rel='http://schemas.google.com/g/2005#home' address='danrowe1978@gmail.com' primary='true'/>
            var emails = from xElement in xContactEntry.XPathSelectElements("gd:email", Ns)
                         select new ContactAssociation(ToContactAssociation(xElement.Attribute("rel")), xElement.Attribute("address").Value);
            result.EmailAddresses.AddRange(emails);

            //<gd:phoneNumber rel='http://schemas.google.com/g/2005#mobile' uri='tel:+33-6-43-06-76-58' primary='true'>+33  6 4306 7658</gd:phoneNumber>
            var phoneNumbers = from xElement in xContactEntry.XPathSelectElements("gd:phoneNumber", Ns)
                               select new ContactAssociation(ToContactAssociation(xElement.Attribute("rel")), xElement.Value);
            result.PhoneNumbers.AddRange(phoneNumbers);

            /*<gd:organization rel='http://schemas.google.com/g/2005#work'><gd:orgName>Technip</gd:orgName></gd:organization>*/
            var organizations = from xElement in xContactEntry.XPathSelectElements("gd:organization", Ns)
                                select new ContactAssociation(ToContactAssociation(xElement.Attribute("rel")), xElement.XPathSelectElement("gd:orgName", Ns).Value);
            result.Organizations.AddRange(organizations);

            //<gContact:relation rel='partner'>Anne</gContact:relation>
            var relationships = from xElement in xContactEntry.XPathSelectElements("gContact:relation", Ns)
                                select new ContactAssociation(ToContactAssociation(xElement.Attribute("rel")), xElement.Value);
            result.Relationships.AddRange(relationships);

            return result;
        }

        private static XName ToXName(string prefix, string name)
        {
            var xNamespace = Ns.LookupNamespace(prefix);
            if (xNamespace == null)
                throw new InvalidOperationException(prefix + " namespace prefix is not valid");
            return XName.Get(name, xNamespace);
        }

        private static string XPathString(XNode source, string expression, IXmlNamespaceResolver ns)
        {
            var result = source.XPathSelectElement(expression, ns);
            return result == null ? null : result.Value;
        }

        private static string ToContactAssociation(XAttribute relAttribute)
        {
            //Could be a look
            if (relAttribute == null || relAttribute.Value == null)
                return "Other";
            var hashIndex = relAttribute.Value.LastIndexOf("#", StringComparison.Ordinal);
            var association = relAttribute.Value.Substring(hashIndex + 1);
            return PascalCase(association);
        }

        private static string PascalCase(string input)
        {
            var head = input.Substring(0, 1).ToUpperInvariant();
            var tail = input.Substring(1);
            return head + tail;
        }
    }
}
