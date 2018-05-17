using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Model;
using dk.nsi.seal.Model.ModelBuilders;
using NUnit.Framework;
using SealTest.Properties;

namespace SealTest.Model
{
    public class FederationTest : AbstractTest
    {
        [Test]
        public void IsTrustedStsCertificateTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactoryWithTestFederation(Global.MocesCprGyldig);

            //Create IdCard
            UserIdCard idCard = CreateIdCardForSTS(factory);

            //Sign IdCard
            idCard.Sign<Assertion>(factory.SignatureProvider);

            UserIdCard idc = (UserIdCard)SealUtilities.SignIn(idCard, "NETS DANID A/S", Settings.Default.SecurityTokenService);

            //Assert that STS certificate goes through
            Assert.DoesNotThrow(delegate { idc.ValidateSignatureAndTrust(factory.Federation); });
        }

        [Test]
        public void SelfSignedIdCardTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactoryWithTestFederation(Global.MocesCprGyldig);

            //Create IdCard
            UserIdCard idCard = CreateIdCardForSTS(factory);

            //Sign IdCard
            idCard.Sign<Assertion>(factory.SignatureProvider);

            //Assert that selfsigned idCard fails
            Assert.Throws<ModelException>(delegate { idCard.ValidateSignatureAndTrust(factory.Federation); });
        }

        [Test]
        public void SimpleChainTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactoryWithTestFederation(Global.MocesCprGyldig);
            bool validation = factory.Federation.IsValidCertificate(Global.MocesCprGyldig);

            Assert.True(validation);
        }

        [Test]
        public void ExpiredCertificateTest()
        {
            X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\FOCES_udloebet.p12", "Test1234");

            SOSIFactory factory = CreateSOSIFactoryWithSosiFederation(Global.MocesCprGyldig);
            bool validation = factory.Federation.IsValidCertificate(newCert);

            Assert.False(validation);
        }

        [Test]
        public void RevokedCertificateTest()
        {
            X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\oces2\\PP\\MOCES_spaerret.p12", "Test1234");

            SOSIFactory factory = CreateSOSIFactoryWithTestFederation(Global.MocesCprGyldig);
            bool validation = factory.Federation.IsValidCertificate(newCert);

            Assert.False(validation);
        }

        [Test]
        public void InvalidChainSTestFederationTest()
        {
            X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\SelfSigned.pfx", "Test1234");

            SOSIFactory factory = CreateSOSIFactoryWithTestFederation(Global.MocesCprGyldig);

            Assert.Throws<CryptographicException>(delegate { factory.Federation.IsValidCertificate(newCert); });
        }

        [Test]
        public void InvalidChainSosiFederationTest()
        {
            X509Certificate2 newCert = new X509Certificate2(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Resources\\SelfSigned.pfx", "Test1234");

            SOSIFactory factory = CreateSOSIFactoryWithSosiFederation(Global.MocesCprGyldig);

            Assert.Throws<CryptographicException>(delegate { factory.Federation.IsValidCertificate(newCert); });
        }

        [Test]
        public void SosiFederationTest()
        {
            //Create factory
            SOSIFactory factory = CreateSOSIFactoryWithSosiFederation(Global.MocesCprGyldig);

            //Create IdCard
            UserIdCard idCard = CreateIdCardForSTS(factory);

            //Sign IdCard
            idCard.Sign<Assertion>(factory.SignatureProvider);

            UserIdCard idc = (UserIdCard)SealUtilities.SignIn(idCard, "NETS DANID A/S", Settings.Default.SecurityTokenService);

            //Assert that STS certificate fails due to mismatch in prefix/cvr
            Assert.Throws<ModelException>(delegate { idc.ValidateSignatureAndTrust(factory.Federation); });
        }
    }
}
