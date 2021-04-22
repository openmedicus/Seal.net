using System.Security.Cryptography.X509Certificates;
using dk.nsi.seal;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Factories;
using dk.nsi.seal.Federation;
using dk.nsi.seal.pki;
using dk.nsi.seal.Vault;
using NUnit.Framework.Internal;

namespace SealTest.Model
{
    public abstract class AbstractTest
    {
        public SOSIFactory CreateSOSIFactory(X509Certificate2 cert)
        {
            GenericCredentialVault vault = new GenericCredentialVault();

            //Make sure certStore is cleaned for testing
            RemoveAllCerts(vault);

            //Add test certificate to vault
            X509Certificate2 newCert = cert;
            //newCert.FriendlyName = vault.ALIAS_SYSTEM;
            vault.AddTrustedCertificate(newCert);

            CredentialVaultSignatureProvider sigProvider = new CredentialVaultSignatureProvider(vault);
            SOSIFactory factory = new SOSIFactory(null, sigProvider);
            return factory;
        }

        public SOSIFactory CreateSOSIFactoryWithTestFederation(X509Certificate2 cert)
        {
            SosiTestFederation federation = new SosiTestFederation(new CrlCertificateStatusChecker());
            GenericCredentialVault vault = new GenericCredentialVault();

            //Make sure certStore is cleaned for testing
            RemoveAllCerts(vault);

            //Add test certificate to vault
            X509Certificate2 newCert = cert;
            //newCert.FriendlyName = vault.ALIAS_SYSTEM;
            vault.AddTrustedCertificate(newCert);

            CredentialVaultSignatureProvider sigProvider = new CredentialVaultSignatureProvider(vault);
            SOSIFactory factory = new SOSIFactory(federation, sigProvider);
            return factory;
        }

        public SOSIFactory CreateSOSIFactoryWithSosiFederation(X509Certificate2 cert)
        {
            SosiFederation federation = new SosiFederation(new CrlCertificateStatusChecker());
            GenericCredentialVault vault = new GenericCredentialVault();

            //Make sure certStore is cleaned for testing
            RemoveAllCerts(vault);

            //Add test certificate to vault
            X509Certificate2 newCert = cert;
            //newCert.FriendlyName = vault.ALIAS_SYSTEM;
            vault.AddTrustedCertificate(newCert);

            CredentialVaultSignatureProvider sigProvider = new CredentialVaultSignatureProvider(vault);
            SOSIFactory factory = new SOSIFactory(federation, sigProvider);
            return factory;
        }

        public void RemoveAllCerts(GenericCredentialVault vault)
        {
            var certStore = vault.CertStore;
            certStore.Open(OpenFlags.ReadWrite);
            foreach (var cer in certStore.Certificates)
            {
                vault.RemoveTrustedCertificate(cer.SerialNumber);
            }
            certStore.Close(); ;
        }

        public UserIdCard CreateMocesUserIdCard(SOSIFactory factory)
        {
            return factory.CreateNewUserIdCard("Sygdom.dk", new UserInfo("1802602810", "Amaja", "Christiansen", "jso@trifork.com", "Læge", "7170", "ZXCVB"), new CareProvider(SubjectIdentifierType.medcomcvrnumber, "30808460", "TRIFORK SERVICES A/S // CVR:25520041"), AuthenticationLevel.MocesTrustedUser, null, null, Global.MocesCprGyldig, null);
        }

        public UserIdCard CreateUserIdCard(SOSIFactory factory, string userName, string passWord)
        {
            return factory.CreateNewUserIdCard("ItSystem", new UserInfo("12345678", "Test", "Person", "test@person.dk", "Tester", "Læge", "12345"), new CareProvider(SubjectIdentifierType.medcomcvrnumber, "25520041", "TRIFORK SERVICES A/S // CVR:25520041"), AuthenticationLevel.UsernamePasswordAuthentication, userName, passWord, factory.GetCredentialVault().GetSystemCredentials(), "alt");
        }

        public SystemIdCard CreateVocesSystemIdCard(SOSIFactory factory)
        {
            return factory.CreateNewSystemIdCard("ItSystem", new CareProvider(SubjectIdentifierType.medcomcvrnumber, "25520041", "TRIFORK SERVICES A/S // CVR:25520041"), AuthenticationLevel.VocesTrustedSystem, null, null, factory.GetCredentialVault().GetSystemCredentials(), "alt");
        }

        public UserIdCard CreateIdCardForSTS(SOSIFactory factory)
        {
            return factory.CreateNewUserIdCard("Sygdom.dk", new UserInfo("1802602810", "Stine", "Svendsen", "stineSvendsen@example.com", "læge", "7170", "ZXCVB"), new CareProvider(SubjectIdentifierType.medcomcvrnumber, "30808460", "Statens Serum Institut"), AuthenticationLevel.MocesTrustedUser, "", "", Global.MocesCprGyldig, "");
        }
    }
}
