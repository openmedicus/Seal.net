using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
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
