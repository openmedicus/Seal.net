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
            writer.WriteAttributeString("id", id.ToString("D"));

            writer.WriteStartElement("Timestamp", ns.wsu);
            writer.WriteElementString("Created", ns.wsu, createdTime.ToString("u").Replace(' ', 'T'));
            writer.WriteEndElement();

            sc.Xassertion.WriteTo(writer);
        }

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return ns.wsse; }
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
