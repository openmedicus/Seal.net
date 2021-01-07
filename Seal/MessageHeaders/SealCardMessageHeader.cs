using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace dk.nsi.seal
{
    public class SealCardMessageHeader : MessageHeader
    {
        public SealCard sc;
        public Guid id;
        public DateTime createdTime;

        public SealCardMessageHeader()
        {
            id = Guid.NewGuid();
            var n = DateTime.Now;
            createdTime = new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
        }

        public SealCardMessageHeader(SealCard sc):this()
        {
            this.sc = sc;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            //Id is not allowed on security element
            //writer.WriteAttributeString("id", id.ToString("D"));

            writer.WriteStartElement("Timestamp", NameSpaces.wsu);
            writer.WriteElementString("Created", NameSpaces.wsu, createdTime.ToString("u").Replace(' ', 'T'));
            writer.WriteEndElement();

            sc.Xassertion.WriteTo(writer);
        }

        public override string Name => "Security";

        public override string Namespace => NameSpaces.wsse;

        public override bool MustUnderstand => false;
    }
}
