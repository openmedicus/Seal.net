using System.IdentityModel.Tokens;

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