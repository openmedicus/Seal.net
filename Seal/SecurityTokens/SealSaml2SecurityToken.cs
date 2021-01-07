using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    class SealSaml2SecurityToken : SecurityToken
    {
        public XElement assertion;
        public SealSaml2SecurityToken(XElement assertion)
        {
            this.assertion = assertion;
        }

        public override string Id
        {
            get { return "IDCard"; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return null; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                if (assertion == null) return DateTime.MaxValue;
                return DateTime.Parse(assertion.Element(NameSpaces.xsaml + "Conditions").Attribute("NotBefore").Value);
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                if (assertion == null) return DateTime.MinValue;
                return DateTime.Parse(assertion.Element(NameSpaces.xsaml + "Conditions").Attribute("NotOnOrAfter").Value);
            }
        }
    }
}