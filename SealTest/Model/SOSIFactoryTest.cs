using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Model.ModelBuilders;
using NUnit.Framework;

namespace SealTest.Model
{
    [TestFixture]
    public class SOSIFactoryTest : AbstractTest
    {
        [Test]
        public void CreateIdCardTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard
            SystemIdCard idCard = factory.CreateNewSystemIdCard("ItSystem", new CareProvider(SubjectIdentifierType.medcomitsystemname, "TestSystem", "Trifork"), AuthenticationLevel.UsernamePasswordAuthentication, "user", "test123", null, "alt");
            Assert.NotNull(idCard);
        }

        [Test]
        public void DeserializeUnsignedUserIdCardTest()
        {
            //Create Factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard
            UserIdCard idCard = CreateMocesUserIdCard(factory);

            Assertion assertion = idCard.GetAssertion<Assertion>();

            UserIdCard deserializedCard = (UserIdCard)factory.DeserializeIdCard(assertion);

            //Assert they are equal
            Assert.True(idCard.CreatedDate == deserializedCard.CreatedDate);
            Assert.True(idCard.ExpiryDate == deserializedCard.ExpiryDate);
            Assert.True(idCard.IsValidInTime == deserializedCard.IsValidInTime);
            Assert.True(idCard.UserInfo.Equals(deserializedCard.UserInfo));
            Assert.True(idCard.AuthenticationLevel.Equals(deserializedCard.AuthenticationLevel));
            Assert.True(idCard.CertHash == deserializedCard.CertHash);
            Assert.True(idCard.AlternativeIdentifier == deserializedCard.AlternativeIdentifier);
            Assert.True(idCard.IdCardId == deserializedCard.IdCardId);
            Assert.True(idCard.Issuer == deserializedCard.Issuer);
            Assert.True(idCard.Username == deserializedCard.Username);
            Assert.True(idCard.Password == deserializedCard.Password);
            Assert.True(idCard.SystemInfo.ItSystemName == deserializedCard.SystemInfo.ItSystemName);
            Assert.True(idCard.SystemInfo.CareProvider.Equals(deserializedCard.SystemInfo.CareProvider));
            Assert.True(idCard.Version == deserializedCard.Version);
            Assert.Throws<ModelBuildException>(delegate { var cert = deserializedCard.SignedByCertificate; });
        }

        [Test]
        public void DeserializeUnsignedSystemIdCardTest()
        {
            //Create Factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard
            SystemIdCard idCard = CreateVocesSystemIdCard(factory);

            Assertion assertion = idCard.GetAssertion<Assertion>();

            SystemIdCard deserializedCard = (SystemIdCard)factory.DeserializeIdCard(assertion);

            //Assert they are equal
            Assert.True(idCard.CreatedDate == deserializedCard.CreatedDate);
            Assert.True(idCard.ExpiryDate == deserializedCard.ExpiryDate);
            Assert.True(idCard.IsValidInTime == deserializedCard.IsValidInTime);
            Assert.True(idCard.AuthenticationLevel.Equals(deserializedCard.AuthenticationLevel));
            Assert.True(idCard.CertHash == deserializedCard.CertHash);
            Assert.True(idCard.AlternativeIdentifier == deserializedCard.AlternativeIdentifier);
            Assert.True(idCard.IdCardId == deserializedCard.IdCardId);
            Assert.True(idCard.Issuer == deserializedCard.Issuer);
            Assert.True(idCard.Username == deserializedCard.Username);
            Assert.True(idCard.Password == deserializedCard.Password);
            Assert.True(idCard.SystemInfo.ItSystemName == deserializedCard.SystemInfo.ItSystemName);
            Assert.True(idCard.SystemInfo.CareProvider.Equals(deserializedCard.SystemInfo.CareProvider));
            Assert.True(idCard.Version == deserializedCard.Version);
            Assert.Throws<ModelBuildException>(delegate { var cert = deserializedCard.SignedByCertificate; });
        }

        [Test]
        public void DeserializeSignedSystemIdCardTest()
        {
            //Create Factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard
            SystemIdCard idCard = CreateVocesSystemIdCard(factory);
            idCard.Sign<Assertion>(factory.SignatureProvider);

            Assertion assertion = idCard.GetAssertion<Assertion>();

            SystemIdCard deserializedCard = (SystemIdCard)factory.DeserializeIdCard(assertion);

            //Assert they are equal
            Assert.True(idCard.CreatedDate == deserializedCard.CreatedDate);
            Assert.True(idCard.ExpiryDate == deserializedCard.ExpiryDate);
            Assert.True(idCard.IsValidInTime == deserializedCard.IsValidInTime);
            Assert.True(idCard.AuthenticationLevel.Equals(deserializedCard.AuthenticationLevel));
            Assert.True(idCard.CertHash == deserializedCard.CertHash);
            Assert.True(idCard.AlternativeIdentifier == deserializedCard.AlternativeIdentifier);
            Assert.True(idCard.IdCardId == deserializedCard.IdCardId);
            Assert.True(idCard.Issuer == deserializedCard.Issuer);
            Assert.True(idCard.Username == deserializedCard.Username);
            Assert.True(idCard.Password == deserializedCard.Password);
            Assert.True(idCard.SystemInfo.ItSystemName == deserializedCard.SystemInfo.ItSystemName);
            Assert.True(idCard.SystemInfo.CareProvider.Equals(deserializedCard.SystemInfo.CareProvider));
            Assert.True(idCard.Version == deserializedCard.Version);
            Assert.True(idCard.SignedByCertificate.Equals(deserializedCard.SignedByCertificate));
        }

        [Test]
        public void DeserializeSignedUserIdCardTest()
        {
            //Create Factory
            SOSIFactory factory = CreateSOSIFactory(Global.MocesCprGyldig);

            //Create IdCard
            UserIdCard idCard = CreateMocesUserIdCard(factory);
            idCard.Sign<Assertion>(factory.SignatureProvider);

            Assertion assertion = idCard.GetAssertion<Assertion>();

            UserIdCard deserializedCard = (UserIdCard)factory.DeserializeIdCard(assertion);

            //Assert they are equal
            Assert.True(idCard.CreatedDate == deserializedCard.CreatedDate);
            Assert.True(idCard.ExpiryDate == deserializedCard.ExpiryDate);
            Assert.True(idCard.IsValidInTime == deserializedCard.IsValidInTime);
            Assert.True(idCard.UserInfo.Equals(deserializedCard.UserInfo));
            Assert.True(idCard.AuthenticationLevel.Equals(deserializedCard.AuthenticationLevel));
            Assert.True(idCard.CertHash == deserializedCard.CertHash);
            Assert.True(idCard.AlternativeIdentifier == deserializedCard.AlternativeIdentifier);
            Assert.True(idCard.IdCardId == deserializedCard.IdCardId);
            Assert.True(idCard.Issuer == deserializedCard.Issuer);
            Assert.True(idCard.Username == deserializedCard.Username);
            Assert.True(idCard.Password == deserializedCard.Password);
            Assert.True(idCard.SystemInfo.ItSystemName == deserializedCard.SystemInfo.ItSystemName);
            Assert.True(idCard.SystemInfo.CareProvider.Equals(deserializedCard.SystemInfo.CareProvider));
            Assert.True(idCard.Version == deserializedCard.Version);
            Assert.True(idCard.SignedByCertificate.Equals(deserializedCard.SignedByCertificate));
        }
    }
}
