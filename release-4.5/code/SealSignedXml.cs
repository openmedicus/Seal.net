using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    class SealSignedXml : SignedXml
    {
        public XmlDocument xml { get; private set; }

        public SealSignedXml(string xml)
            : this(stringToXml(xml))
        {
        }

        public SealSignedXml(Stream stream)
            : this(streamToXml(stream))
        {
        }

        public SealSignedXml(XDocument xml)
            : this(streamToXml(XDocToStream(xml)))
        {
        }

        public SealSignedXml(XElement xml)
            : this(streamToXml(XDocToStream(xml)))
        {
        }

        public SealSignedXml(XmlDocument xml)
            : base(xml)
        {
            this.xml = xml;
        }

        static XmlDocument stringToXml(string xml)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);
            return doc;
        }

        static XmlDocument streamToXml(Stream stream)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(stream);
            return doc;
        }

        static Stream XDocToStream(XDocument xml)
        {
            var ms = new MemoryStream();
            xml.Save(ms, SaveOptions.DisableFormatting);
            ms.Position = 0;
            return ms;
        }

        static Stream XDocToStream(XElement xml)
        {
            var ms = new MemoryStream();
            xml.Save(ms, SaveOptions.DisableFormatting);
            ms.Position = 0;
            return ms;
        }

        public bool CheckEnvelopeSignature()
        {
            var nsManager = ns.MakeNsManager(xml.NameTable);
            var sig = xml.SelectSingleNode("/soap:Envelope/soap:Header/wsse:Security/ds:Signature", nsManager) as XmlElement;
            LoadXml(sig);
            var cert = KeyInfo.Cast<KeyInfoX509Data>().Select(d => d.Certificates[0] as X509Certificate2).Where(c => c != null).FirstOrDefault();
            return CheckSignature(cert, true);
        }

        public bool CheckAssertionSignature()
        {
            var nsManager = ns.MakeNsManager(xml.NameTable);
            var xmlass = xml.DocumentElement.LocalName == "Assertion" ? xml.DocumentElement : xml.GetElementsByTagName("Assertion", ns.saml)[0] as XmlElement;
            var sig = xmlass.GetElementsByTagName("Signature", ns.ds)[0] as XmlElement;
            if( sig == null ) return false;
            LoadXml(sig);
            var cert = KeyInfo.Cast<KeyInfoX509Data>().Select(d => d.Certificates[0] as X509Certificate2).Where(c => c != null).FirstOrDefault();
            return CheckSignature(cert, true);
        }

        public XmlDocument Sign(X509Certificate2 cert)
        {
            var refnames = new string[] { "#timestamp", "#messageID", "#action", "#body" };
            foreach (string s in refnames)
            {
                var reference = new Reference();
                reference.Uri = s;
                reference.AddTransform(new XmlDsigExcC14NTransform());
                AddReference(reference);
            }

            SigningKey = cert.PrivateKey;
            SignedInfo.CanonicalizationMethod = new XmlDsigExcC14NTransform().Algorithm;
            KeyInfo = new KeyInfo();
            KeyInfo.AddClause(new KeyInfoX509Data(cert));

            ComputeSignature();

            XmlElement signaelm = GetXml();
            var XSecurity = xml.SelectSingleNode("/soap:Envelope/soap:Header/wsse:Security", ns.MakeNsManager(xml.NameTable)) as XmlElement;
            XSecurity.AppendChild(signaelm);

            return xml;
        }

        public XmlElement GetDGWSSign(X509Certificate2 cert)
        {
            var refnames = new string[] { "#IDCard" };
            foreach (string s in refnames)
            {
                var reference = new Reference();
                reference.Uri = s;
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigExcC14NTransform());
                AddReference(reference);
            }

            SigningKey = cert.PrivateKey;
            SignedInfo.CanonicalizationMethod = new XmlDsigExcC14NTransform().Algorithm;
            KeyInfo = new KeyInfo();
            KeyInfo.AddClause(new KeyInfoX509Data(cert));

            ComputeSignature();
            return GetXml();
        }

        public override XmlElement GetIdElement(XmlDocument doc, string id)
        {
            var idElem = doc.SelectSingleNode("//*[@wsu:Id=\"" + id + "\"]", ns.MakeNsManager(doc.NameTable)) as XmlElement;
            var tid = idElem ?? base.GetIdElement(doc, id);
            return tid;
        }
    }
}
