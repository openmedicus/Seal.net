using System.IdentityModel.Protocols.WSTrust;
using System.Xml;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    class ResponseSerializer : WSTrust13ResponseSerializer
    {
        public override void ReadXmlElement(XmlReader reader, RequestSecurityTokenResponse rstr, WSTrustSerializationContext context)
        {
            if (reader.LocalName == "RequestedSecurityToken")
            {
                var rd = reader.ReadSubtree();
                rd.ReadToFollowing("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
                var assdoc = XDocument.Load(rd.ReadSubtree());

                rstr.RequestedSecurityToken = new RequestedSecurityToken( new SealSaml2SecurityToken(assdoc.Root));
                rstr.Properties.Add(NameSpaces.DGWSAssertion, assdoc);
            }
            else
            {
                base.ReadXmlElement(reader, rstr, context);
            }
        }
    }
}