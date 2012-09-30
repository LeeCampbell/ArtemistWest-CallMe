using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using ArtemisWest.CallMe.Contract;
using ArtemisWest.CallMe.Contract.Contacts;
using ArtemisWest.CallMe.Google.Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArtemisWest.CallMe.Google.UnitTests
{
    [TestClass]
    public class GoogleContactQueryProviderFixture
    {
        private IContact _actual;

        [TestInitialize]
        public void SetUp()
        {
            var auth = new StubAuthModel();
            var web = new StubWebrequestService();
            var sut = new GoogleContactQueryProvider(auth, web, new StubLoggerFactory());
            var activeProfile = new Profile(new[] { new PersonalIdentifier(null, null, "dummy") });

            web.Response = File.ReadAllText(@"ExampleFullContactQueryResponse.xml");
            _actual = sut.Search(activeProfile).First();
        }

        [TestMethod]
        public void Should_return_Title_from_response_xml()
        {
            Assert.AreEqual("Daniel Rowe", _actual.Title);
        }

        [TestMethod]
        public void Should_return_FullName_from_response_xml()
        {
            Assert.AreEqual("Daniel MiddleName Rowe", _actual.FullName);
        }

        [TestMethod]
        public void Should_return_Birthday_from_response_xml()
        {
            Assert.AreEqual(new DateTime(1978, 02, 15), _actual.DateOfBirth);
        }

        [TestMethod]
        public void Should_return_ImageLink_from_response_xml()
        {
            var expected = new UriBuilder(@"https://www.google.com/m8/feeds/photos/media/lee.ryan.campbell%40gmail.com/2b")
                               {
                                   Query = @"access_token=SomeToken"
                               };
            Assert.AreEqual(expected.Uri, _actual.Image);
        }

        [TestMethod]
        public void Should_return_Emails_from_response_xml()
        {
            /* <gd:email rel='http://schemas.google.com/g/2005#home' address='danrowe1978@gmail.com' primary='true'/>
             * <gd:email rel='http://schemas.google.com/g/2005#work' address='DRowe@technip.com'/>
             */

            var emailAddresses = _actual.EmailAddresses.ToList();
            Assert.AreEqual(2, emailAddresses.Count());
            Assert.AreEqual("Home", emailAddresses[0].Association);
            Assert.AreEqual("danrowe1978@gmail.com", emailAddresses[0].Name);
            Assert.AreEqual("Work", emailAddresses[1].Association);
            Assert.AreEqual("DRowe@technip.com", emailAddresses[1].Name);
        }

        [TestMethod]
        public void Should_return_PhoneNumbers_from_response_xml()
        {
            /* <gd:phoneNumber rel='http://schemas.google.com/g/2005#mobile' uri='tel:+33-6-43-06-76-58' primary='true'>+33  6 4306 7658</gd:phoneNumber>*/

            var phoneNumbers = _actual.PhoneNumbers.ToList();
            Assert.AreEqual(1, phoneNumbers.Count());
            Assert.AreEqual("Mobile", phoneNumbers[0].Association);
            Assert.AreEqual("+33  6 4306 7658", phoneNumbers[0].Name);
        }

        [TestMethod]
        public void Should_return_Organizations_from_response_xml()
        {
            /*<gd:organization rel='http://schemas.google.com/g/2005#work'><gd:orgName>Technip</gd:orgName></gd:organization>*/

            var organizations = _actual.Organizations.ToList();
            Assert.AreEqual(1, organizations.Count());
            Assert.AreEqual("Work", organizations[0].Association);
            Assert.AreEqual("Technip", organizations[0].Name);
        }

        [TestMethod]
        public void Should_return_Relationships_from_response_xml()
        {
            //<gContact:relation rel='partner'>Anne</gContact:relation>

            var relationships = _actual.Relationships.ToList();
            Assert.AreEqual(1, relationships.Count());
            Assert.AreEqual("Partner", relationships[0].Association);
            Assert.AreEqual("Anne", relationships[0].Name);
        }
    }
}