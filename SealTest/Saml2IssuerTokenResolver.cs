using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace SealTest
{
    class Saml2IssuerTokenResolver : IssuerTokenResolver
    {
        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            key = null;
            var kn = keyIdentifierClause as X509RawDataKeyIdentifierClause;
            if (kn == null) return false;
            var cert = new X509Certificate2(kn.GetX509RawData());
            if (cert == null) return false;
            key = new X509AsymmetricSecurityKey(cert);
            return key != null;
        }
    }

}
