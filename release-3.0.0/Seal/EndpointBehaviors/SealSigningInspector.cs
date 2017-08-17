using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    class SealSigningInspector : IClientMessageInspector
    {
        public ClientCredentials clientCredentials;

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            XmlDocument request = correlationState as XmlDocument;

            if (!reply.IsFault)
            {
                MessageBuffer msgbuf = reply.CreateBufferedCopy(int.MaxValue);
                var signcheck = new SealSignedXml(msgbuf.AsStream());
                if (!signcheck.CheckEnvelopeSignature())
                {
                    throw new Exception("Response signature Error");
                }
                reply = msgbuf.CreateMessage();
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (clientCredentials == null || clientCredentials.ClientCertificate.Certificate == null) throw new Exception("clientCredentials Certificate is missing");
            string action = null, messageID = "urn:uuid:" + Guid.NewGuid().ToString("D");

            foreach (var head in request.Headers)
            {
                var x = XElement.Parse(head.ToString());
                switch (head.Name)
                {
                    case "Action": action = x.Value; break;
                    case "MessageID": messageID= x.Value; break;
                }
            }

            MessageBuffer msgbuf = request.CreateBufferedCopy(int.MaxValue);
            var xdoc = XDocument.Load(msgbuf.AsStream());
            SealUtilities.CheckAndSetSamlDsPreFix(xdoc); //Hack 

            //Fill header
            ns.SetMissingNamespaces(xdoc);
            var hd = xdoc.Root.Element(ns.xsoap + "Header");
            var ac = hd.Element(ns.xwsa2 + "Action") ?? hd.Element(ns.xwsa + "Action");
            var md = hd.Element(ns.xwsa2 + "MessageID") ?? hd.Element(ns.xwsa + "MessageID");

            hd.Add(new XElement(ns.xwsa + "Action", new XAttribute("mustUnderstand", "1"), new XAttribute(ns.xwsu + "Id", "action"), action),
                new XElement(ns.xwsa + "MessageID", new XAttribute(ns.xwsu + "Id", "messageID"), messageID),
                new XElement(ns.xwsse + "Security", new XAttribute("mustUnderstand", "1"), new XAttribute(ns.xwsu + "Id", "security"),
                    new XElement(ns.xwsu + "Timestamp", new XAttribute(ns.xwsu + "Id", "timestamp"),
                        new XElement(ns.xwsu + "Created", DateTime.UtcNow.ToString("u").Replace(' ', 'T'))
                        )
                    )
                );
            ac.Remove();
            if (md != null) md.Remove();

            xdoc.Root.Element(ns.xsoap + "Body").Add(new XAttribute(ns.xwsu + "Id", "body"));

            var signer = new SealSignedXml(xdoc);
            XmlDocument envelope = signer.Sign(clientCredentials.ClientCertificate.Certificate);

            var nrd = new XmlNodeReader(envelope);
            msgbuf = Message.CreateMessage(nrd, int.MaxValue, request.Version).CreateBufferedCopy(int.MaxValue);
            request = msgbuf.CreateMessage();
            return envelope;
        }
    }
}