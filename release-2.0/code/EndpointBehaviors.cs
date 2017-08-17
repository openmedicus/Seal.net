using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace dk.nsi.seal
{
    public class SealSigningEndpointBehavior : IEndpointBehavior
    {
        ClientCredentials clientCredentials;

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            clientCredentials = bindingParameters.OfType<ClientCredentials>().FirstOrDefault() ??
#if NET35
            endpoint.Behaviors.OfType<ClientCredentials>().FirstOrDefault();
#else
            endpoint.EndpointBehaviors.OfType<ClientCredentials>().FirstOrDefault();
#endif

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
#if NET35
            var collection = clientRuntime.MessageInspectors;
#else
            var collection = clientRuntime.ClientMessageInspectors;
#endif
            collection.Add(new SealSigningInspector { clientCredentials = clientCredentials });
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            throw new NotImplementedException();
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class SealSigningBehaviorExtentionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SealSigningEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SealSigningEndpointBehavior();
        }
    }

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
#if NET35
            var xdoc = XDocUtil.Load(msgbuf.AsStream());
#else
            var xdoc = XDocument.Load(msgbuf.AsStream());
#endif
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


    public class SealEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
#if NET35
            var collection = clientRuntime.MessageInspectors;
#else
            var collection = clientRuntime.ClientMessageInspectors;
#endif
            collection.Add(new SealMessageInspect());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new SealMessageInspect());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class SealBehaviorExtentionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SealEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SealEndpointBehavior();
        }
    }

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
#if NET35
                var xdoc = XDocUtil.Load(request.ToStream());
#else
                var xdoc = XDocument.Load(request.ToStream());
#endif
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


    public class SosiGatewayEndpointBehavior : IEndpointBehavior
    {
        public string To;

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
#if NET35
            var collection = clientRuntime.MessageInspectors;
#else
            var collection = clientRuntime.ClientMessageInspectors;
#endif
            collection.Add(new SosiGatewayMessageInspect() { To = To });
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class SosiGatewayBehaviorExtentionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SosiGatewayEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SosiGatewayEndpointBehavior() { To = toUri };
        }

        [ConfigurationProperty("toUri")]
        public string toUri
        {
            get
            {
                return (string)base["toUri"];
            }
            set
            {
                base["toUri"] = value;
            }
        }
    }

    class SosiGatewayMessageInspect : IClientMessageInspector
    {
        static XNamespace xsoap = "http://schemas.xmlsoap.org/soap/envelope/", xsosi = "http://sosi.dk/gw/2007.09.01",
                          xds = "http://www.w3.org/2000/09/xmldsig#", xdgws = "http://www.medcom.dk/dgws/2006/04/dgws-1.0.xsd";

        public string To;
        //TODO kommentar
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var h = request.Headers;
            var idx = request.Headers.FindHeader("Action", "http://schemas.microsoft.com/ws/2005/05/addressing/none");
            if (idx != -1)
            {
                var action = request.Headers.GetHeader<string>(idx);
                request.Headers.RemoveAt(idx);
                request.Headers.Add(MessageHeader.CreateHeader("Action", "http://www.w3.org/2005/08/addressing", action));
            }
            request.Headers.Add(MessageHeader.CreateHeader("To", "http://www.w3.org/2005/08/addressing", To));

            return null;
        }
        //TODO kommentar
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

                var q = from h in xdoc.Root.Elements(xsoap + "Header")
                        from i in h.Elements(xsosi + "implicitLoginHeader")
                        from r in i.Elements(xsosi + "requestIdCardDigestForSigningResponse")
                        select new SosiGWLoginError()
                        {
                            DigestValue = r.Element(xds + "DigestValue").Value,
                            BrowserUrl = r.Element(xsosi + "BrowserUrl").Value
                        };

                var le = q.FirstOrDefault();
                var reason = xdoc.Root.Elements(xsoap + "Body")
                                    .Elements(xsoap + "Fault")
                                    .Elements("faultstring")
                                    .Select(v => v.Value).FirstOrDefault();
                if (le != null)
                    throw new FaultException<SosiGWLoginError>(le, new FaultReason(reason));

                var dgwsfc = xdoc.Root.Descendants(xdgws + "FaultCode").FirstOrDefault();
                if (dgwsfc != null)
                {
                    var fs = xdoc.Root.Descendants("faultstring").First();
                    throw new FaultException<XElement>(dgwsfc, new FaultReason(fs.Value), new FaultCode("Server"));
                }
                reply = msgbuf.CreateMessage();
            }
        }
    }

//    public class SosiGatewayBehavior : IEndpointBehavior
//    {
//        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
//        {
//        }

//        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
//        {
//#if NET35
//            var collection = clientRuntime.MessageInspectors;
//#else
//            var collection = clientRuntime.ClientMessageInspectors;
//#endif
//            collection.Add(new SosiGatewayMessageInspect());
//        }

//        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
//        {
//        }

//        public void Validate(ServiceEndpoint endpoint)
//        {
//        }
//    }

//    public class SosiGatewayBehaviorExtentionElement : BehaviorExtensionElement
//    {
//        public override Type BehaviorType
//        {
//            get { return typeof(SosiGatewayBehavior); }
//        }

//        protected override object CreateBehavior()
//        {
//            return new SosiGatewayBehavior();
//        }
//    }

//    public class SosiGatewayMessageInspect : IClientMessageInspector
//    {
//        public void AfterReceiveReply(ref Message reply, object correlationState)
//        {
//            if (reply.IsFault)
//            {
//                MessageBuffer msgbuf = reply.CreateBufferedCopy(int.MaxValue);
//                Message tmpMessage = msgbuf.CreateMessage();

//                var xdoc = new XDocument();
//                using (var wr = xdoc.CreateWriter())
//                {
//                    tmpMessage.WriteBody(wr);
//                }

//                var q = from h in xdoc.Root.Elements(ns.xsoap + "Header")
//                        from i in h.Elements(ns.xsosi + "implicitLoginHeader")
//                        from r in i.Elements(ns.xsosi + "requestIdCardDigestForSigningResponse")
//                        select new SosiGWLoginError()
//                        {
//                            DigestValue = r.Element(ns.xds + "DigestValue").Value,
//                            BrowserUrl = r.Element(ns.xsosi + "BrowserUrl").Value
//                        };

//                var le = q.FirstOrDefault();
//                var reason = xdoc.Root.Elements(ns.xsoap + "Body")
//                                    .Elements(ns.xsoap + "Fault")
//                                    .Elements("faultstring")
//                                    .Select(v => v.Value).FirstOrDefault();
//                if (le != null)
//                    throw new FaultException<SosiGWLoginError>(le, new FaultReason(reason));

//                var dgwsfc = xdoc.Root.Descendants(ns.xdgws + "FaultCode").FirstOrDefault();
//                if (dgwsfc != null)
//                {
//                    var fs = xdoc.Root.Descendants("faultstring").First();
//                    throw new FaultException<XElement>(dgwsfc, new FaultReason(fs.Value), new FaultCode("Server"));
//                }
//                reply = msgbuf.CreateMessage();
//            }
//        }

//        public object BeforeSendRequest(ref Message request, IClientChannel channel)
//        {
//            return null;
//        }
//    }

    static class MessageEx
    {
        public static Stream AsStream(this MessageBuffer msgbuf)
        {
            Message tmpMessage = msgbuf.CreateMessage();

            var stream = new MemoryStream();
            using (var wr = XmlWriter.Create(stream))
            {
                tmpMessage.WriteMessage(wr);
            }
            stream.Position = 0;
            return stream;
        }

        public static Stream ToStream(this Message msg)
        {
            MessageBuffer msgbuf = msg.CreateBufferedCopy(int.MaxValue);
            Message tmpMessage = msgbuf.CreateMessage();

            var stream = new MemoryStream();
            using (var wr = XmlWriter.Create( stream) )
            {
                tmpMessage.WriteMessage(wr);
            }
            stream.Position = 0;
            return stream;
        }

        public static Message ToMessage(this XDocument xdoc, MessageVersion mv)
        {
            using (var ms = new MemoryStream())
            {
                xdoc.Save(ms, SaveOptions.DisableFormatting);
                ms.Position = 0;
                using (var reader = XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max))
                {
                    var msgbuf = Message.CreateMessage(reader, int.MaxValue, mv).CreateBufferedCopy(int.MaxValue);
                    return msgbuf.CreateMessage();
                }
            }

        }    
    }
}
