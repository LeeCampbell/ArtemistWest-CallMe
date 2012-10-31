using System.IO;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Google.Contacts;
using NUnit.Framework;

namespace ArtemisWest.CallMe.Google.UnitTests
{
    [TestFixture]
    public class GoogleIdentityProviderFixture
    {
        [Test]
        public void Should_return_Profile_from_request_xml()
        {
            var auth = new StubAuthModel();
            var web = new StubWebrequestService();
            var logFactory = new StubLoggerFactory();
            var sut = new GoogleIdentityProvider(auth, web, logFactory);

            web.Response =File.ReadAllText(@"ExampleFullContactQueryResponse.xml");
            var profile = sut.FindProfile(new []{""}).First();

            Assert.AreEqual(3, profile.Identifiers.Count);
            Assert.AreEqual("danrowe1978@gmail.com", profile.Identifiers[0].Value);
            Assert.AreEqual("DRowe@technip.com", profile.Identifiers[1].Value);
            Assert.AreEqual("+33  6 4306 7658", profile.Identifiers[2].Value);
        }
    }
}
