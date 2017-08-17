using System.ServiceModel.Channels;
using System.Xml;

namespace dk.nsi.seal
{
    public class DgwsMessageHeader : MessageHeader
    {
        public DgwsHeader sc;

        public DgwsMessageHeader()
        {
        }

        public DgwsMessageHeader(DgwsHeader sc)
        {
            this.sc = sc;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            foreach (var elm in sc.data.Elements())
            {
                elm.WriteTo(writer);
            }
        }

        public override string Name
        {
            get { return "Header"; }
        }

        public override string Namespace
        {
            get { return "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd"; }
        }

        public override bool MustUnderstand
        {
            get
            {
                return false;
            }
        }
    }
}