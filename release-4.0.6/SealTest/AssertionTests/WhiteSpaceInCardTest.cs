using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using NUnit.Framework;
using SealTest.AssertionTests.AssertionBuilders;
using SealTest.NSTWsProvider;
using SealTest.Properties;

namespace SealTest.AssertionTests
{
    [TestFixture]
    public class WhiteSpaceInCardTest
    {
        private readonly X509Certificate2 _userCertificate = new X509Certificate2(
            $"{TestContext.CurrentContext.TestDirectory}/Resources/certificates/Sonja_Bach_Laege.p12", "Test1234");

        /// <summary>
        /// 1. Creating NemID as a Saml2Assertion
        /// 2. Exchanging the Saml2Assertion for a Sosi Card, created by the STS
        /// 3. Calling the National Test Service - NTS
        /// </summary>
        [Test]
        public void TestIDcard_Does_not_change_whiteSpace_Saml2SosiStsClient()
        {
            var nemidAssertion = NemIdAssertionBuilder.MakeNemIdAssertion(
                _userCertificate,
                Global.StatensSerumInstitutFoces,
                "0309691444",
                "Sonja",
                "Bach",
                "Sonja_Bach@nsi.dk",
                "7170",
                "3",
                "46837428",
                "Statens Serum Institut",
                "NS363");

            var sealCard = ExchangeNemLoginAssertionForSosiSTSCard("NS363", nemidAssertion, _userCertificate);

            CallNts(sealCard);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(sealCard.Xassertion));
        }

        /// <summary>
        /// 1. Creating a locally signed Seal Card
        /// 2. Getting STS to sign the card.
        /// 3. Calling the National Test Service - NTS
        /// </summary>
        [Test]
        public void TestIDcard_Does_not_change_whiteSpace()
        {
            var localSealCard = SealCard.Create(AssertionMaker.MakeAssertionForSTS(Global.MocesCprGyldig));

            var sosiCardSTS = SealUtilities.SignIn(localSealCard, "http://www.ribeamt.dk/EPJ", Settings.Default.SecurityTokenService);

            CallNts(sosiCardSTS);

            Assert.IsTrue(SealUtilities.CheckAssertionSignature(localSealCard.Xassertion));
            Assert.IsTrue(SealUtilities.CheckAssertionSignature(sosiCardSTS.Xassertion));
        }

        public static SealCard ExchangeNemLoginAssertionForSosiSTSCard(string userAuthorizationCode, Saml2Assertion nemidAssertion, X509Certificate2 cert)
        {
            using (var stsClient = new Saml2SosiStsClient("sts_OIOSaml2Sosi"))
            {
                stsClient.ChannelFactory.Credentials.ClientCertificate.Certificate = cert;

                var healthContextAssertion = SealUtilities.MakeHealthContextAssertion(
                    "Test Sundhed.dk",
                    Global.StatensSerumInstitutFoces.SubjectName.Name,
                    "Sygdom.dk",
                    userAuthorizationCode);

                return stsClient.ExchangeAssertion(nemidAssertion, healthContextAssertion, "http://sosi.dk");
            }
        }

        private static void CallNts(SealCard sealCard)
        {
            var client = new NtsWSProviderClient();

            client.Endpoint.EndpointBehaviors.Add(new SealEndpointBehavior());

            using (new OperationContextScope(client.InnerChannel))
            {
                var header = new Header
                {
                    SecurityLevel = 4,
                    SecurityLevelSpecified = true,
                    Linking = new Linking
                    {
                        MessageID = Guid.NewGuid().ToString("D")
                    }
                };

                // Adding seal-security and dgws-header soap header
                OperationContext.Current.OutgoingMessageHeaders.Add(new SealCardMessageHeader(sealCard));
                OperationContext.Current.OutgoingMessageHeaders.Add(new DgwsMessageHeader(DgwsHeader.Create(header)));

                client.invoke("test");
            }
        }

        public class SealCardMessageHeaderHeader : MessageHeader
        {
            private readonly XElement _header;

            public SealCardMessageHeaderHeader(XElement header)
            {
                _header = header;
            }

            public override string Name => "Header";

            public override string Namespace => "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd";

            public override bool MustUnderstand => false;

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                foreach (var descendant in _header.Nodes())
                {
                    descendant.WriteTo(writer);
                }
            }
        }
    }
}