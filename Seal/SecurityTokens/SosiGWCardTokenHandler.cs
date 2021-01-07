using System;
using Microsoft.IdentityModel.Tokens;
using System.Xml;
using dk.nsi.seal.Constants;

namespace dk.nsi.seal
{
    public class SosiGWCardTokenHandler : SecurityTokenHandler
    {
        public string[] GetTokenTypeIdentifiers()
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
            writer.WriteStartElement("SecurityTokenReference", NameSpaces.wsse);
            writer.WriteAttributeString("URI","#IDCard" );
            writer.WriteEndElement();
        }

        public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters)
        {
            throw new NotImplementedException();
        }
    }
}