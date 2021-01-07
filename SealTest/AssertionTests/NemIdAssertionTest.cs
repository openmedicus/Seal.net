using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using dk.nsi.seal;
using Microsoft.IdentityModel.Tokens.Saml2;
using NUnit.Framework;
using SealTest.AssertionTests.AssertionBuilders;
using SealTest.MedicineCardService;

namespace SealTest.AssertionTests
{
    [TestFixture]
    public class NemIdAssertionTest
    {
        /**
         * In this example the user has a single authorization in the authorization register. The id card request is
         * made for that one specifik authorization.
         */

        [Test]
        public void UserWithOneSpecificAuthorization()
        {
            const string keystorePath = "Karl_Hoffmann_Svendsen_Laege.p12";
            const string userCpr = "0102732379";
            const string userGivenName = "Karl Hoffmann";
            const string userSurName = "Svendsen";
            const string userEmail = "Karl_Hoffmann_Svendsen@nsi.dk";
            const string userRole = "7170";
            const string userAuthorizationCode = "NS362";

            var idCard = TestNemId2SealAssertion(keystorePath, userCpr, userGivenName, userSurName, userEmail, userRole, userAuthorizationCode);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(idCard.Xassertion));
            Assert.IsNotNull(idCard.Id, "No user information found");
            //assertEquals("Incorrect authorization code", "NS362", idCard.getUserInfo().getAuthorizationCode());
            //assertEquals("Incorrect education code", "7170", idCard.getUserInfo().getRole());
        }

        /**
         * In this example the user has a single authorization in the authorization register. The id card request is
         * made with only a limitations on the role/education code. This is only possible because the user has one
         * and only one authorization with the "Doctor" role.
         */

        [Test]
        public void UserWithOneDoctorAuthorization()
        {
            const string keystorePath = "Karl_Hoffmann_Svendsen_Laege.p12";
            const string userCpr = "0102732379";
            const string userGivenName = "Karl Hoffmann";
            const string userSurName = "Svendsen";
            const string userEmail = "Karl_Hoffmann_Svendsen@nsi.dk";
            const string userRole = "7170";
            const string userAuthorizationCode = null;

            var idCard = TestNemId2SealAssertion(keystorePath, userCpr, userGivenName, userSurName, userEmail, userRole, userAuthorizationCode);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(idCard.Xassertion));
            //assertNotNull("No user information found", idCard.getUserInfo());
            //assertEquals("Incorrect authorization code", "NS362", idCard.getUserInfo().getAuthorizationCode());
            //assertEquals("Incorrect education code", "7170", idCard.getUserInfo().getRole());
        }

        /**
         * In this example the user has a single authorization in the authorization register. The id card request is
         * made without a limitations on the authorization that will be used. This is only possible because the user
         * has one and only one authorization.
         */

        [Test]
        public void UserWithOneAuthorization()
        {
            const string keystorePath = "Karl_Hoffmann_Svendsen_Laege.p12";
            const string userCpr = "0102732379";
            const string userGivenName = "Karl Hoffmann";
            const string userSurName = "Svendsen";
            const string userEmail = "Karl_Hoffmann_Svendsen@nsi.dk";
            const string userRole = "IGNORED"; // Must not be an empty string or null, but all values that are not four digits are ignored by STS
            const string userAuthorizationCode = null;

            var idCard = TestNemId2SealAssertion(keystorePath, userCpr, userGivenName, userSurName, userEmail, userRole, userAuthorizationCode);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(idCard.Xassertion));
            //assertNotNull("No user information found", idCard.getUserInfo());
            //assertEquals("Incorrect authorization code", "NS362", idCard.getUserInfo().getAuthorizationCode());
            //assertEquals("Incorrect education code", "7170", idCard.getUserInfo().getRole());
        }


        /**
         * In this example the user has two authorizations in the authorization register. The id card request is
         * made for one specifik authorization. If no limitations where made in the request, the call would fail
         * because STS will not choose one at random for us.
         */

        [Test]
        public void UserWithSeveralAuthorization()
        {
            const string keystorePath = "Sonja_Bach_Laege.p12";
            const string userCpr = "0309691444";
            const string userGivenName = "Sonja";
            const string userSurName = "Bach";
            const string userEmail = "Sonja_Bach@nsi.dk";
            const string userRole = "7170";
            const string userAuthorizationCode = "NS363";

            var idCard = TestNemId2SealAssertion(keystorePath, userCpr, userGivenName, userSurName, userEmail, userRole, userAuthorizationCode);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(idCard.Xassertion));
            //assertNotNull("No user information found", idCard.getUserInfo());
            //assertEquals("Incorrect authorization code", "NS363", idCard.getUserInfo().getAuthorizationCode());
            //assertEquals("Incorrect education code", "7170", idCard.getUserInfo().getRole());
        }

        /**
         * In this example the user has no authorization in the authorization register. The id card request is made
         * without a limitations on the authorization that will be used. The returned id card does not have an
         * authorization in it, and it's usage might be limited.
         */

        [Test]
        [Ignore("EducationCode must be specified or unique authorization must exist.")]
        public void UserWithNoAuthorization()
        {
            const string keystorePath = "Brian_Moeller_Laege.p12";
            const string userCpr = "1103811325";
            const string userGivenName = "Brian";
            const string userSurName = "Møller";
            const string userEmail = "Brian_Moeller@nsi.dk";
            const string userRole = "IGNORED"; // Must not be an empty string or null, but all values that are not four digits are ignored by STS
            const string userAuthorizationCode = null;

            var idCard = TestNemId2SealAssertion(keystorePath, userCpr, userGivenName, userSurName, userEmail, userRole, userAuthorizationCode);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(idCard.Xassertion));
            //assertNotNull("No user information found", idCard.getUserInfo());
            //assertNull("No authorization code was expected", idCard.getUserInfo().getAuthorizationCode());
            //assertEquals("No education code was expected", "IGNORED", idCard.getUserInfo().getRole());
        }

        public static SealCard TestNemId2SealAssertion(string keystorePath,
            string userCpr,
            string userGivenName,
            string userSurName,
            string userEmail,
            string userRole,
            string userAuthorizationCode)
        {
            //Opretter et NemID som Saml2Assertion
            //Veksler til Sosi kort
            //Kalder lokal service med Sosikort

            var nemidAssertion = NemIdAssertionBuilder.MakeNemIdAssertion(
                new X509Certificate2(TestContext.CurrentContext.TestDirectory + "/Resources/certificates/" + keystorePath, "Test1234"),
                Global.StatensSerumInstitutFoces,
                userCpr,
                userGivenName,
                userSurName,
                userEmail,
                userRole, "3", "46837428", "Statens Serum Institut",
                userAuthorizationCode);

            var sc = ExchangeNemLoginAssertionForSosiSTSCard(userAuthorizationCode, nemidAssertion);

            var client = new MedicineCardPortTypeClient("FMKTestEnv");

            var getMedicineCardRequest20150601 = FMKRequestMother.GetMedicineCardRequest20150601(userCpr, sc);
            var presStatus = new PrescriptionReplicationStatusType();
            var response = new MedicineCardType[1];
            var res = client.GetMedicineCard_2015_06_01(getMedicineCardRequest20150601.Security, getMedicineCardRequest20150601.Header, getMedicineCardRequest20150601.OnBehalfOf, getMedicineCardRequest20150601.WhitelistingHeader, getMedicineCardRequest20150601.ConsentHeader, getMedicineCardRequest20150601.GetMedicineCardRequest, out presStatus, out response);

            return sc;
        }

        public static SealCard ExchangeNemLoginAssertionForSosiSTSCard(string userAuthorizationCode, Saml2Assertion nemidAssertion)
        {
            using (var stsClient = new Saml2SosiStsClient("sts_OIOSaml2Sosi"))
            {
                //stsClient.ChannelFactory.Credentials.ClientCertificate.Certificate = Global.StatensSerumInstitutFoces;

                var healthContextAssertion = SealUtilities.MakeHealthContextAssertion(
                    "Test Sundhed.dk",
                    Global.StatensSerumInstitutFoces.SubjectName.Name,
                    "Sygdom.dk", userAuthorizationCode);

                return stsClient.ExchangeAssertion(nemidAssertion, healthContextAssertion, "http://sosi.dk");
            }
        }
    }
}