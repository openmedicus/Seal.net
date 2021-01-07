using System;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    [Obsolete("Deprecated, please use IdCard")]
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
            return new SealCard(SerializerUtil.Serialize(assertion).Root);
        }

        public T GetAssertion<T>() where T : class
        {
            return SerializerUtil.Deserialize<T>(Xassertion);
        }

        public T GetAssertion<T>(string rootid) where T : class
        {
            return SerializerUtil.Deserialize<T>(Xassertion, rootid);
        }

        public string Id => "IDCard";

        public DateTime ValidFrom => DateTime.Parse(Xassertion.Element(NameSpaces.xsaml + "Conditions").Attribute("NotBefore").Value);
 
        public DateTime ValidTo => DateTime.Parse(Xassertion.Element(NameSpaces.xsaml + "Conditions").Attribute("NotOnOrAfter").Value);
    }
}