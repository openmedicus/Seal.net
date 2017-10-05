using System;
using System.IdentityModel.Tokens;
using System.Xml;

namespace dk.nsi.seal
{
    class Saml2SecurityToken2TokenHandler : Saml2SecurityTokenHandler
    {
        Saml2AssertionSerializer ser = new Saml2AssertionSerializer();

        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            var t = token as Saml2SecurityToken2;
            base.WriteToken(writer, token);
            ser.WriteSaml2Assertion(writer, t.health);
        }

        public override Type TokenType
        {
            get { return typeof(Saml2SecurityToken2); }
        }
    }
}