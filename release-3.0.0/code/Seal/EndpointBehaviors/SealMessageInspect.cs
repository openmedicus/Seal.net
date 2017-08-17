using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    class SealMessageInspect : IDispatchMessageInspector, IClientMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var xdoc = new XDocument();
            using (var wr = xdoc.CreateWriter())
            {
                wr.WriteStartElement("DGWSInfo");
                request.Headers.WriteHeader(request.Headers.FindHeader("Security", ns.wsse), wr);
                request.Headers.WriteHeader(request.Headers.FindHeader("Header", ns.dgws), wr);
                wr.WriteEndElement();
            }

            var err = SealUtilities.ValidateSecurity(xdoc.Root.Descendants(ns.xwsse + "Security").FirstOrDefault());

            if (err != null)
            {
                xdoc.Root.Add( new XElement( "Fault", 
                    new XElement( "reason", err.Item1),
                    new XElement( "detail", err.Item2))
                    ); 
                
                throw new FaultException<string>(xdoc.ToString(SaveOptions.DisableFormatting), new FaultReason("requesterror"));
            }
            return xdoc;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var reqxdoc = correlationState as XDocument;

            if (reply.IsFault) //replaces s:Server with Server in faultcode
            {
                MessageBuffer msgbuf = reply.CreateBufferedCopy(int.MaxValue);
                var r = msgbuf.CreateMessage();

                var xdoc = new XDocument();
                using (var wr = xdoc.CreateWriter())
                {
                    r.WriteMessage(wr);
                }

                if (xdoc.Descendants("faultstring").First().Value == "requesterror")
                {
                    var fc = xdoc.Descendants(ns.xdgws + "FaultCode").First();
                    reqxdoc = XDocument.Parse(fc.Value);
                    var xfault = reqxdoc.Root.Element("Fault");
                    xdoc.Descendants("faultstring").First().Value = xfault.Element("reason").Value;
                    fc.Value = xfault.Element("detail").Value;
                }

                xdoc.Descendants("faultcode").First().Value = "Server";
                using (var reader = xdoc.CreateReader())
                {
                    var rmsg = Message.CreateMessage(reader, int.MaxValue, reply.Version).CreateBufferedCopy(int.MaxValue);
                    reply = rmsg.CreateMessage();
                }
            }

            if (reqxdoc != null)
            {
                var msg = reqxdoc.Descendants(ns.xdgws + "MessageID").First();
                reqxdoc.Descendants(ns.xdgws + "Linking").First().Add(new XElement(ns.xdgws + "RequireNonRepudiationReceipt", msg.Value));
                msg.Value = Guid.NewGuid().ToString("D");

                reply.Headers.Add(new DgwsMessageHeader(new DgwsHeader(reqxdoc.Root.Element(ns.xdgws + "Header"))));
                reqxdoc.Descendants(ns.xdgws + "FlowStatus").First().Value = "flow_finalized_succesfully";
            }
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.IsFault)
            {
                MessageBuffer msgbuf = reply.CreateBufferedCopy(int.MaxValue);
                Message tmpMessage = msgbuf.CreateMessage();

                var xdoc = new XDocument();
                using (var wr = xdoc.CreateWriter())
                {
                    tmpMessage.WriteBody(wr);
                }

                var q = from h in xdoc.Root.Elements(ns.xsoap + "Header")
                    from i in h.Elements(ns.xsosi + "implicitLoginHeader")
                    from r in i.Elements(ns.xsosi + "requestIdCardDigestForSigningResponse")
                    select new SosiGWLoginError()
                    {
                        DigestValue = r.Element(ns.xds + "DigestValue").Value,
                        BrowserUrl = r.Element(ns.xsosi + "BrowserUrl").Value
                    };

                var le = q.FirstOrDefault();
                var reason = xdoc.Root.Elements(ns.xsoap + "Body")
                    .Elements(ns.xsoap + "Fault")
                    .Elements("faultstring")
                    .Select(v => v.Value).FirstOrDefault();
                if (le != null)
                    throw new FaultException<SosiGWLoginError>(le, new FaultReason(reason));

                var dgwsfc = xdoc.Root.Descendants(ns.xdgws + "FaultCode").FirstOrDefault();
                if (dgwsfc != null)
                {
                    var fs = xdoc.Root.Descendants("faultstring").First();
                    throw new FaultException<XElement>(dgwsfc, new FaultReason(fs.Value), new FaultCode("Server"));
                }

                reply = msgbuf.CreateMessage();
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var hdidx = request.Headers.FindHeader("Security", ns.wsse);
            if (hdidx == -1) throw new FaultException("Security header is missing");
            var hd = request.Headers[hdidx];

            if (!(hd is SealCardMessageHeader))
            {
                var xdoc = XDocument.Load(request.ToStream());
                SealUtilities.CheckAndSetSamlDsPreFix(xdoc); //Hack 
                request = xdoc.ToMessage(request.Version);
            }
            else
            {
                request.Headers.RemoveAt(hdidx);
                request.Headers.Insert(hdidx, hd as SealCardMessageHeader);
            }
            return null;
        }
    }
}