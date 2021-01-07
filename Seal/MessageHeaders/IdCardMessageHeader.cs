using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace dk.nsi.seal.MessageHeaders
{
    public class IdCardMessageHeader : MessageHeader
    {
        public IdCard sc;
        public Guid id;
        public DateTime createdTime;

        public IdCardMessageHeader()
        {
            id = Guid.NewGuid();
            var n = DateTime.Now;
            createdTime = new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
        }

        public IdCardMessageHeader(IdCard sc):this()
        {
            this.sc = sc;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteAttributeString("id", id.ToString("D"));

            writer.WriteStartElement("Timestamp", NameSpaces.wsu);
            writer.WriteElementString("Created", NameSpaces.wsu, createdTime.ToString("u").Replace(' ', 'T'));
            writer.WriteEndElement();

            sc.Xassertion.WriteTo(writer);
        }

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return NameSpaces.wsse; }
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
