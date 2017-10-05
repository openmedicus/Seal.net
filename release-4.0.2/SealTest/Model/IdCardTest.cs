using System;
using System.Security.Cryptography.X509Certificates;
using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Model;
using NUnit.Framework;
using System.Configuration;
using System.Linq;

namespace SealTest.Model
{
    public class IdCardTest : AbstractTest
    {
        [Test]
        public void IdCardValidatorTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard with missing UserGivenName
            UserIdCard idCard = factory.CreateNewUserIdCard("ItSystem", new UserInfo("12345678", null, "Person", "test@person.dk", "Tester", "Læge", "12345"), new CareProvider(SubjectIdentifierType.medcomcvrnumber, "25520041", "TRIFORK SERVICES A/S // CVR:25520041"), AuthenticationLevel.MocesTrustedUser, "", "", factory.GetCredentialVault().GetSystemCredentials(), "alt");

            //Try to sign the idCard
            Assert.Throws<ModelException>(delegate { idCard.Sign<Assertion>(factory.SignatureProvider); });
        }

        [Test]
        public void IdCardNullUserInfoTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard with missing UserInfo
            Assert.Throws<ModelException>(delegate {
                                                       UserIdCard idCard = factory.CreateNewUserIdCard("ItSystem", null, new CareProvider(SubjectIdentifierType.medcomcvrnumber, "25520041", "TRIFORK SERVICES A/S // CVR:25520041"), AuthenticationLevel.MocesTrustedUser, "", "", factory.GetCredentialVault().GetSystemCredentials(), "alt");
            });
        }

        [Test]
        public void IdCardNullSystemInfoTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard with missing UserInfo
            Assert.Throws<ModelException>(delegate {
                                                       factory.CreateNewSystemIdCard("", new CareProvider(SubjectIdentifierType.medcomcvrnumber, "25520041", "TRIFORK SERVICES A/S // CVR:25520041"), AuthenticationLevel.MocesTrustedUser, null, null, factory.GetCredentialVault().GetSystemCredentials(), "alt");
            });
        }

        [Test]
        public void IdCardNullCareProviderTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard with missing UserInfo
            Assert.Throws<ModelException>(delegate {
                factory.CreateNewSystemIdCard("ItSystem", null, AuthenticationLevel.MocesTrustedUser, null, null, factory.GetCredentialVault().GetSystemCredentials(), "alt");
            });
        }

        [Test]
        public void IdCardUserNamePassTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard with username/password
            UserIdCard idCard = CreateUserIdCard(factory,"user","test123");

            //Get Assertion
            Assertion ass = idCard.GetAssertion<Assertion>();

            Assert.True(ass.Subject.SubjectConfirmation.SubjectConfirmationData.Item.GetType() == typeof(UsernameToken));

            //Assert assertion was created succesfully
            Assert.NotNull(ass);
            Assert.NotNull(idCard.Xassertion);
        }

        [Test]
        public void IdCardMocesSignTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard
            UserIdCard idCard = CreateMocesUserIdCard(factory);

            //Sign IdCard
            Assertion ass = idCard.Sign<Assertion>(factory.SignatureProvider);
            Assertion ass2 = idCard.GetAssertion<Assertion>();

            //Assert assertion was created succesfully
            Assert.NotNull(ass);
            Assert.NotNull(idCard.Xassertion);

            //Make sure the assertion returned from Sign and Get are the same.
            Assert.True(ass.Signature.SignatureValue.ToString() == ass2.Signature.SignatureValue.ToString());
        }

        [Test]
        public void ValidateSignatureTest()
        {
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);
            UserIdCard idCard = CreateMocesUserIdCard(factory);
            idCard.Sign<Assertion>(factory.SignatureProvider);

            //This throws if you are not connected to VPN
            Assert.DoesNotThrow(delegate { idCard.ValidateSignatureAndTrust(factory.GetCredentialVault()); });
        }

        [Test]
        public void ValidateSignatureNegativeTest()
        {
			if(ConfigurationManager.AppSettings.AllKeys.Contains("CheckDate"))
			{
				ConfigurationManager.AppSettings["CheckDate"] = "True";
			}
            //Get invalid certificate
            X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\MOCES_udloebet.p12", "Test1234");
            SOSIFactory factory = CreateSOSIFactory(newCert);
            UserIdCard idCard = CreateMocesUserIdCard(factory);
            idCard.Sign<Assertion>(factory.SignatureProvider);

            Assert.Throws<ModelException>(delegate { idCard.ValidateSignatureAndTrust(factory.GetCredentialVault()); });
			if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckDate"))
			{
				ConfigurationManager.AppSettings["CheckDate"] = "False";
			}
		}

		[Test]
        public void IdCardVocesSignTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.VocesGyldig);

            //Create IdCard
            SystemIdCard idCard = CreateVocesSystemIdCard(factory);

            //Sign IdCard
            Assertion ass = idCard.Sign<Assertion>(factory.SignatureProvider);
            Assertion ass2 = idCard.GetAssertion<Assertion>();

            //Assert assertion was created succesfully
            Assert.NotNull(ass);
            Assert.NotNull(idCard.Xassertion);

            //Make sure the assertion returned from Sign and Get are the same.
            Assert.True(ass.Signature.SignatureValue.ToString() == ass2.Signature.SignatureValue.ToString());
        }
    }
}
