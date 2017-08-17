using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Xml;

namespace dk.nsi.seal
{
    class Saml2ResponseSerializer : WSTrust13ResponseSerializer
    {
        Saml2AssertionSerializer ser = new Saml2AssertionSerializer();

        public override void ReadXmlElement(XmlReader reader, RequestSecurityTokenResponse rstr, WSTrustSerializationContext context)
        {
            if (reader.LocalName == "RequestedSecurityToken")
            {
                var rd = reader.ReadSubtree();
                rd.ReadToFollowing("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
                rstr.RequestedSecurityToken = new RequestedSecurityToken(new Saml2SecurityToken(ser.ReadSaml2Assertion(rd.ReadSubtree())));
            }
            else
            {
                base.ReadXmlElement(reader, rstr, context);
            }
        }
    }
}