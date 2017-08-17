using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Vault
{
    public interface ICredentialVault
    {
        string ALIAS_SYSTEM { get; }
        X509Store CertStore { get; }

        X509Certificate2 GetSystemCredentials();
        bool IsTrustedCertificate(X509Certificate2 certificate);
        void SetSystemCredentials(X509Certificate2 cert);
    }
}
