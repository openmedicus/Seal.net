using System.Xml.Linq;

namespace dk.nsi.seal
{
    public class DgwsHeader
    {
        public XElement data;

        public DgwsHeader()
        {
        }

        public DgwsHeader(XElement data)
        {
            this.data = data;
        }

        public static DgwsHeader Create<T>(T dgwsHeader)
        {
            return new DgwsHeader(SealUtilities.Serialize(dgwsHeader).Root);
        }
    }
}