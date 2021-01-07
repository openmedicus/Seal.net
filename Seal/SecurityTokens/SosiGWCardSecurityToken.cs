using System;
using System.IdentityModel.Tokens;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    class SosiGWCardSecurityToken : SecurityToken
    {
        public XElement assertion;
        public SosiGWCardSecurityToken(XElement assertion)
        {
            this.assertion = assertion;
        }

        public override string Id
        {
            get { return "IDCard"; }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return null; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                return DateTime.Parse(assertion.Element(NameSpaces.xsaml + "Conditions").Attribute("NotBefore").Value);
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                return DateTime.Parse(assertion.Element(NameSpaces.xsaml + "Conditions").Attribute("NotOnOrAfter").Value);
            }
        }
    }
}