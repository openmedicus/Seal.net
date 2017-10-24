using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace dk.nsi.seal
{
	public class Saml2AssertionSerializer : Saml2SecurityTokenHandler
    {
        public Saml2AssertionSerializer()
        {
            Configuration = new SecurityTokenHandlerConfiguration()
            {
                IssuerTokenResolver = new Saml2IssuerTokenResolver()
            };
        }

        public Saml2AssertionSerializer(SamlSecurityTokenRequirement samlSecurityTokenRequirement): base(samlSecurityTokenRequirement)
        {
            Configuration = new SecurityTokenHandlerConfiguration()
            {
                IssuerTokenResolver = new Saml2IssuerTokenResolver()
            };
        }

        public Saml2Assertion ReadSaml2Assertion(XmlReader rd)
        {
            return base.ReadAssertion(rd);
        }

        public void WriteSaml2Assertion(XmlWriter wr, Saml2Assertion sa)
        {
            base.WriteAssertion(wr, sa);
        }
    }
    
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
