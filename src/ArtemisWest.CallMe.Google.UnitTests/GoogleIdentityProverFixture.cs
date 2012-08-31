using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Google.Authorization;
using ArtemisWest.CallMe.Google.Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArtemisWest.CallMe.Google.UnitTests
{
    [TestClass]
    public class GoogleIdentityProverFixture
    {
        [TestMethod]
        public void Should_return_Profile_from_request_xml()
        {
            var auth = new StubAuthModel();
            var web = new StubWebrequestService();
            var sut = new GoogleIdentityProvider(auth, web);

            web.Response =File.ReadAllText(@"ExampleFullContactQueryResponse.xml");
            var profile = sut.FindProfile("").First();

            Assert.AreEqual(3, profile.Identifiers.Count);
            Assert.AreEqual("danrowe1978@gmail.com", profile.Identifiers[0].Value);
            Assert.AreEqual("DRowe@technip.com", profile.Identifiers[1].Value);
            Assert.AreEqual("+33  6 4306 7658", profile.Identifiers[2].Value);
        }
    }   
    public class StubWebrequestService : IWebRequstService
    {
        public string Response { get; set; }
        public IObservable<string> GetResponse(WebRequestParameters requestParameters)
        {
            return Observable.Return(Response);
        }
    }
    public class StubAuthModel : IAuthorizationModel
    {
        private IObservable<AuthorizationStatus> _status;
        private IResourceScope[] _availableServices;
        private ObservableCollection<IResourceScope> _selectedServices;

        public IObservable<AuthorizationStatus> Status
        {
            get { return _status; }
        }

        public IResourceScope[] AvailableServices
        {
            get { return _availableServices; }
        }

        public ObservableCollection<IResourceScope> SelectedServices
        {
            get { return _selectedServices; }
        }

        public void RegisterAuthorizationCallback(RequestAuthorizationCode callback)
        {
            throw new NotImplementedException();
        }

        public IObservable<string> RequestAccessToken()
        {
            return Observable.Return("SomeToken");
        }
    }
}
