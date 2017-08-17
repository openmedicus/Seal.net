using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#if NET35
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
#else
using System.IdentityModel.Tokens;
#endif



namespace dk.nsi.seal
{
    public class SealSecurityToken : SecurityToken
    {
        public SealCard sealCard;
        public SealSecurityToken(XElement assertion)
        {
            this.sealCard = new SealCard { Xassertion = assertion };
        }

        public SealSecurityToken(SealCard sealCard)
        {
            this.sealCard = sealCard;
        }

        public override string Id
        {
            get { return sealCard.Id; }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return null; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                return sealCard.ValidFrom;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                return sealCard.ValidTo;
            }
        }
    }

    public class SealSecurityTokenHandler : SecurityTokenHandler
    {
        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[] { Constants.SealSecurityTokenHandlerId };
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
    }


    class Saml2SecurityToken2 : Saml2SecurityToken
    {
        public Saml2Assertion health;
        public Saml2SecurityToken2(Saml2Assertion org, Saml2Assertion health)
            : base(org)
        {
            this.health = health;
        }
    }

    class Saml2SecurityToken2TokenHandler : Saml2SecurityTokenHandler
    {
        Saml2AssertionSerializer ser = new Saml2AssertionSerializer();

        public override void WriteToken(XmlWriter writer, SecurityToken token)
        {
            var t = token as Saml2SecurityToken2;
            base.WriteToken(writer, token);
            ser.WriteSaml2Assertion(writer, t.health);
        }

        public override Type TokenType
        {
            get { return typeof(Saml2SecurityToken2); }
        }
    }


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

        public override System.Collections.ObjectModel.ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return null; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                if (assertion == null) return DateTime.MaxValue;
                return DateTime.Parse(assertion.Element(ns.xsaml + "Conditions").Attribute("NotBefore").Value);
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                if (assertion == null) return DateTime.MinValue;
                return DateTime.Parse(assertion.Element(ns.xsaml + "Conditions").Attribute("NotOnOrAfter").Value);
            }
        }
    }

    
    public class SealSaml2SecurityTokenHandler : SecurityTokenHandler
    {
        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[] { Constants.DGWSSecurityTokenHandlerId };
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
                return DateTime.Parse(assertion.Element(ns.xsaml + "Conditions").Attribute("NotBefore").Value);
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                return DateTime.Parse(assertion.Element(ns.xsaml + "Conditions").Attribute("NotOnOrAfter").Value);
            }
        }
    }

    public class SosiGWCardTokenHandler : SecurityTokenHandler
    {
        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[] { Constants.SosiGWSecurityTokenHandlerId };
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
