using System;
using System.IdentityModel.Tokens;
using System.Xml;
using dk.nsi.seal.Constants;

namespace dk.nsi.seal
{
    public class SosiGWCardTokenHandler : SecurityTokenHandler
    {
        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[] { SecurityTokenConstants.SosiGWSecurityTokenHandlerId };
        }

        public override Type TokenType
        {
            get { return typeof(SosiGWCardSecurityToken); }
        }

        public override bool CanWriteToken
        {
            get
            {
                return true;
            }
        }

        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            writer.WriteStartElement("SecurityTokenReference", ns.wsse);
            writer.WriteAttributeString("URI","#IDCard" );
            writer.WriteEndElement();
        }
    }
}