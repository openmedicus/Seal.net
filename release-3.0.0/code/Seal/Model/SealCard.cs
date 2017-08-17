using System;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    public class SealCard
    {
        public XElement Xassertion;

        public SealCard()
        {
        }

        public SealCard(XElement xassertion)
        {
            Xassertion = xassertion;
        }

        public static SealCard Create<T>(T assertion)
        {
            return new SealCard(SealUtilities.Serialize(assertion).Root);
        }

        public T GetAssertion<T>() where T : class
        {
            return SealUtilities.Deserialize<T>(Xassertion);
        }

        public T GetAssertion<T>(string rootid) where T : class
        {
            return SealUtilities.Deserialize<T>(Xassertion, rootid);
        }

        public string Id
        {
            get { return "IDCard"; }
        }

        public DateTime ValidFrom
        {
            get { return DateTime.Parse(Xassertion.Element(ns.xsaml + "Conditions").Attribute("NotBefore").Value); }
        }

        public DateTime ValidTo
        {
            get { return DateTime.Parse(Xassertion.Element(ns.xsaml + "Conditions").Attribute("NotOnOrAfter").Value); }
        }
    }
}