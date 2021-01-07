using Microsoft.IdentityModel.Protocols.WsTrust;
using System.Xml;
using System.Xml.Linq;
using Microsoft.IdentityModel.Protocols;

namespace dk.nsi.seal
{
    class ResponseSerializer : WsTrustSerializer
    {
        public override Claims ReadClaims(XmlDictionaryReader reader, WsSerializationContext serializationContext)
        {
            return base.ReadClaims(reader, serializationContext);
        }

        /*public override void ReadXmlElement(XmlReader reader, RequestSecurityTokenResponse rstr, WsSerializationContext context)
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
        }*/
    }
}