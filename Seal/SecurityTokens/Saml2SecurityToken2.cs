using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace dk.nsi.seal
{
    class Saml2SecurityToken2 : Saml2SecurityToken
    {
        public Saml2Assertion health;
        public Saml2SecurityToken2(Saml2Assertion org, Saml2Assertion health)
            : base(org)
        {
            this.health = health;
        }
    }
}