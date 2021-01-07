using System;
using System.Security.Authentication;
using dk.nsi.seal.pki;
using dk.nsi.seal.Vault;
using NUnit.Framework;

namespace SealTest.Model
{
    public class VaultTest : AbstractTest
    {
        //VaultTests
        [Test]
        public void SetSystemCredentialsTest()
        {
            var factory = CreateSOSIFactory(Global.MocesCprGyldig);

            var vault = factory.GetCredentialVault();
            //Verify that vault contains system credentials
            Assert.True(vault.GetSystemCredentials().Equals(Global.MocesCprGyldig));

            //Set new system credentials
            vault.SetSystemCredentials(Global.VocesGyldig);
            Assert.True(vault.GetSystemCredentials().Equals(Global.VocesGyldig));
        }

        [Test]
        public void SetSystemCredentialsEmptyStoreTest()
        {
            GenericCredentialVault vault = new GenericCredentialVault();
            RemoveAllCerts(vault);

            vault.SetSystemCredentials(Global.MocesCprGyldig);

            Assert.True(vault.IsTrustedCertificate(Global.MocesCprGyldig));
            Assert.True(vault.GetSystemCredentials().Equals(Global.MocesCprGyldig));
        }

        [Test]
        public void IsTrustedCertTest()
        {
            //string vocesFriendlyName = "NETS DANID A/S - TU VOCES gyldig";
            //Global.VocesGyldig.FriendlyName = vocesFriendlyName;

            string vocesFriendlyName = "5818E231";

            var factory = CreateSOSIFactory(Global.MocesCprGyldig);

            GenericCredentialVault vault = (GenericCredentialVault)factory.GetCredentialVault();

            vault.AddTrustedCertificate(Global.VocesGyldig);

            Assert.True(vault.IsTrustedCertificate(Global.VocesGyldig));
            Assert.True(vault.IsTrustedCertificate(Global.MocesCprGyldig));
            Assert.False(vault.IsTrustedCertificate(Global.cert));

            //Remove VOCES cert
            vault.RemoveTrustedCertificate(vocesFriendlyName);

            //Verify it is no longer trusted
            Assert.False(vault.IsTrustedCertificate(Global.VocesGyldig));
        }

        [Test]
        public void RemoveTrustedCertTest()
        {
            //string vocesFriendlyName = "NETS DANID A/S - TU VOCES gyldig";
            //Global.VocesGyldig.FriendlyName = vocesFriendlyName;
            
            string vocesFriendlyName = "5818E231";

            var factory = CreateSOSIFactory(Global.MocesCprGyldig);

            GenericCredentialVault vault = (GenericCredentialVault)factory.GetCredentialVault();

            //Try to remove non-existing cert
            Assert.Throws<InvalidCredentialException>(delegate { vault.RemoveTrustedCertificate(vocesFriendlyName); });

            vault.AddTrustedCertificate(Global.VocesGyldig);
            //Verify it is now trusted
            Assert.True(vault.IsTrustedCertificate(Global.VocesGyldig));
            Assert.DoesNotThrow(delegate { vault.RemoveTrustedCertificate(vocesFriendlyName); });

            //Verify it is no longer trusted
            Assert.False(vault.IsTrustedCertificate(Global.VocesGyldig));
        }

        [Test]
        public void TestNullVault()
        {
            GenericCredentialVault vault = null;
            Assert.Throws<ArgumentException>(delegate { CredentialVaultSignatureProvider sigProvider = new CredentialVaultSignatureProvider(vault); });
        }
    }
}
