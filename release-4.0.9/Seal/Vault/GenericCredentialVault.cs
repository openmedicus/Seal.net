using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using dk.nsi.seal.Model;
using System.Security.Permissions;

namespace dk.nsi.seal.Vault
{
    public class GenericCredentialVault : ICredentialVault
    {
        public string PROPERTYNAME_CREDENTIAL_VAULT_ALIAS { get { return "credentialVault:alias"; } }
        public const string CREDENTIAL_VAULT_DEFAULT_ALIAS = "SOSI:ALIAS_SYSTEM";
        public string ALIAS_SYSTEM {
            get {
                var alias = CREDENTIAL_VAULT_DEFAULT_ALIAS;
                if (ConfigurationManager.AppSettings.AllKeys.Contains(PROPERTYNAME_CREDENTIAL_VAULT_ALIAS))
                {
                    alias = ConfigurationManager.AppSettings[PROPERTYNAME_CREDENTIAL_VAULT_ALIAS];
                }
                return alias;
            }
        }

        public X509Store CertStore { get; }

		public GenericCredentialVault(string storeName)
		{
			CertStore = new X509Store(storeName, StoreLocation.LocalMachine);
		}

		public GenericCredentialVault(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
			CertStore = new X509Store(storeName, storeLocation);
		}

        public void AddTrustedCertificate(X509Certificate2 cert)
        {
            CertStore.Open(OpenFlags.ReadWrite);
            CertStore.Add(cert);
            CertStore.Close();
        }

        public void RemoveTrustedCertificate(string alias)
        {
            var certToRemove = GetCredentialsByAlias(alias);
            if (certToRemove == null)
            {
                throw new InvalidCredentialException("The supplied alias " + alias + " is not a certificate entry");
            }

            CertStore.Open(OpenFlags.ReadWrite);
            try
            {
                CertStore.Remove(certToRemove);
            }
            catch (Exception e)
            {
                throw new InvalidCredentialException("Unable to remove certificate", e);
            }
            CertStore.Close();
        }

        public X509Certificate2 GetSystemCredentials()
        {
	        return GetCredentialsByAlias(ALIAS_SYSTEM);
        }

        private X509Certificate2 GetCredentialsByAlias(string alias)
        {
            X509Certificate2 resultCert = null;

            CertStore.Open(OpenFlags.ReadOnly);
            foreach (var cert in CertStore.Certificates)
            {
                if (cert.FriendlyName == alias)
                    resultCert = cert;
            }
            CertStore.Close();
            return resultCert;
        }

        public bool IsTrustedCertificate(X509Certificate2 certificate)
        {
	        var found = false;
			CertStore.Open(OpenFlags.ReadOnly);
			foreach (var cert in CertStore.Certificates)
			{
				if (Equals(cert, certificate))
					found = true;
			}
			CertStore.Close();
			return found;
        }

        public void SetSystemCredentials(X509Certificate2 cert)
        {
            cert.FriendlyName = ALIAS_SYSTEM;

            //Remove already existing System Credentials
            var oldSystemCred = GetSystemCredentials();
            CertStore.Open(OpenFlags.ReadWrite);
            if (oldSystemCred != null)
            {
                CertStore.Remove(oldSystemCred);
            }

            //Add new System Credentals
            CertStore.Add(cert);
            CertStore.Close();
        }
    }
}
