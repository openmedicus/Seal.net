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

        public override string Name => "Header";

        public override string Namespace => NameSpaces.dgws;

        public override bool MustUnderstand => false;
    }
}