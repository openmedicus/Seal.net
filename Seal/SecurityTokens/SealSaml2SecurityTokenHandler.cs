using System;
using System.Xml;
using dk.nsi.seal.Constants;
using System.IdentityModel.Tokens;

namespace dk.nsi.seal
{
    public class SealSaml2SecurityTokenHandler : SecurityTokenHandler
    {
        public string[] GetTokenTypeIdentifiers()
        {
            return new string[] { SecurityTokenConstants.DGWSSecurityTokenHandlerId };
        }

        public override Type TokenType
        {
            get { return typeof(SealSaml2SecurityToken); }
        }

        public override bool CanWriteToken
        {
            get
            {
                return true;
            }
        }

        public override void WriteToken(XmlWriter writer, Microsoft.IdentityModel.Tokens.SecurityToken token)
        {
            var t = token as SealSaml2SecurityToken;
            if (t.assertion != null)
            {
                t.assertion.WriteTo(writer);
            }
        }

        public override Microsoft.IdentityModel.Tokens.SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters)
        {
            throw new NotImplementedException();
        }
    }
}