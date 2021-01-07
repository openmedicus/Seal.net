using System;
using Microsoft.IdentityModel.Tokens;
using System.Xml;
using dk.nsi.seal.Constants;

namespace dk.nsi.seal
{
    public class SealSecurityTokenHandler : SecurityTokenHandler
    {
        public string[] GetTokenTypeIdentifiers()
        {
            return new string[] { SecurityTokenConstants.SealSecurityTokenHandlerId };
        }

        public override Type TokenType
        {
            get { return typeof(SealSecurityToken); }
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
            var t = token as SealSecurityToken;
            t.sealCard.Xassertion.WriteTo(writer);
        }

        public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters)
        {
            throw new NotImplementedException();
        }
    }
}