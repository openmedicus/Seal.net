using System;
using System.IdentityModel.Tokens;
using System.Xml;
using dk.nsi.seal.Constants;

namespace dk.nsi.seal
{
    public class SealSaml2SecurityTokenHandler : SecurityTokenHandler
    {
        public override string[] GetTokenTypeIdentifiers()
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

        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            var t = token as SealSaml2SecurityToken;
            if (t.assertion != null)
            {
                t.assertion.WriteTo(writer);
            }
        }
    }
}