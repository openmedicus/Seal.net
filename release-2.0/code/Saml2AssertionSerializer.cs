using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#if NET35
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens;
#endif

namespace dk.nsi.seal
{
    
    internal class Saml2AssertionSerializer : Saml2SecurityTokenHandler
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
