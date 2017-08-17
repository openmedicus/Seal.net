using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using dk.nsi.seal.Constants;

namespace dk.nsi.seal
{
    public class SosiGWLoginError
    {
        public string DigestValue { get; set; }
        public string BrowserUrl { get; set; }
    }

    public class SealUtilities
    {
        private static readonly Dictionary<string, XmlSerializer> serializers = new Dictionary<string, XmlSerializer>();

        //static XmlSerializer getSerializer<T>()
        //{
        //    var t = typeof(T);
        //    var fn = t.FullName;
        //    if (!serializers.ContainsKey(fn))
        //    {
        //        var ta = t.GetCustomAttributes(false).OfType<XmlTypeAttribute>().FirstOrDefault();
        //        if( ta!=null)
        //        {
        //            if (string.IsNullOrEmpty(ta.TypeName))
        //            {
        //                serializers.Add(fn, new XmlSerializer(t, ta.Namespace));
        //            }
        //            else
        //            {
        //                serializers.Add(fn, new XmlSerializer(t, new XmlRootAttribute(ta.TypeName) { Namespace = ta.Namespace }));
        //            }
        //        }
        //        else
        //        {
        //            serializers.Add(fn, new XmlSerializer(t));
        //        }
        //    }
        //    return serializers[fn];
        //}


        private static XmlSerializer getSerializer<T>()
        {
            var t = typeof(T);
            var fn = t.FullName;
            if (!serializers.ContainsKey(fn))
            {
                var rootns = t.GetCustomAttributes(false).OfType<XmlTypeAttribute>().FirstOrDefault().Namespace;
                serializers.Add(fn, new XmlSerializer(t, rootns));
            }
            return serializers[fn];
        }

        private static XmlSerializer getSerializer<T>(string rootName)
        {
            var t = typeof(T);
            var fn = t.FullName + rootName;
            if (!serializers.ContainsKey(fn))
            {
                var rootns = t.GetCustomAttributes(false).OfType<XmlTypeAttribute>().FirstOrDefault().Namespace;
                serializers.Add(fn, new XmlSerializer(t, new XmlRootAttribute(rootName) {Namespace = rootns}));
            }
            return serializers[fn];
        }

        private static Stream Serialize2Stream<T>(T element)
        {
            var ms = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings {Indent = false}))
            {
                getSerializer<T>().Serialize(xmlWriter, element);
            }
            ms.Position = 0;
            return ms;
        }

        internal static XDocument Serialize<T>(T element)
        {
            return XDocument.Load(Serialize2Stream(element), LoadOptions.PreserveWhitespace);
        }

        internal static T Deserialize<T>(XElement document) where T : class
        {
            var ms = new MemoryStream();
            document.Save(ms);
            ms.Position = 0;
            return getSerializer<T>().Deserialize(ms) as T;
        }

        internal static T Deserialize<T>(XElement document, string rootId) where T : class
        {
            var ms = new MemoryStream();
            document.Save(ms);
            ms.Position = 0;
            return getSerializer<T>(rootId).Deserialize(ms) as T;
        }


        internal static T Deserialize<T>(Stream stream) where T : class
        {
            return getSerializer<T>().Deserialize(stream) as T;
        }

        internal static T Deserialize<T>(XmlElement elm) where T : class
        {
            using (var nr = new XmlNodeReader(elm))
            {
                return getSerializer<T>().Deserialize(nr) as T;
            }
        }

        public static T SignAssertion<T>(T element, X509Certificate2 cert) where T : class
        {
            var sxml = new SealSignedXml(Serialize(element));
            var xassertion = sxml.xml.GetElementsByTagName("Assertion", ns.saml)[0] as XmlElement;
            var keyName = xassertion.GetElementsByTagName("KeyName", ns.ds)[0].InnerText;

            var xsignature = sxml.GetDGWSSign(cert);
            xsignature.SetAttribute("id", keyName);
            xassertion.AppendChild(xsignature);

            return Deserialize<T>(sxml.xml.DocumentElement);
        }

        public static bool CheckAssertionSignature<T>(T element)
        {
            var ss = new SealSignedXml(Serialize(element));
            return ss.CheckAssertionSignature();
        }

        public static bool CheckAssertionSignatureNSCheck<T>(T element)
        {
            return CheckAssertionSignatureNSCheck(Serialize(element).Root);
        }

        public static bool CheckAssertionSignatureNSCheck(XElement element)
        {
            var ss = new SealSignedXml(element);
            if (ss.CheckAssertionSignature()) return true;
            SetSamlDsPreFix(element);
            ss = new SealSignedXml(element);
            return ss.CheckAssertionSignature();
        }


        internal static FaultException makeFault(string reason, string detail)
        {
            return new FaultException(MessageFault.CreateFault(new FaultCode("Server"), new FaultReason(reason), detail, new faultwriter()));
        }

        public static FaultException ValidateSecurity<T>(T security) where T : class
        {
            var t = ValidateSecurity(Serialize(security).Root);
            return makeFault(t.Item1, t.Item1);
        }

        public static Tuple<string, string> ValidateSecurity(XElement security)
        {
            if (security == null || (security.Name != ns.xwsse + "Security")) return new Tuple<string, string>("invalid_idcard", "Security element mangler");
            var err = t.check(security, t.sectree);
            if (err != null) return err;

            var scd = security.Descendants(ns.xsaml + "SubjectConfirmationData").First();
            var ki = scd.Element(ns.xds + "KeyInfo");
            if (ki != null)
            {
                if (!CheckAssertionSignatureNSCheck(security))
                {
                    return new Tuple<string, string>("invalid_signature",
                        "Autentifikation eller autorisationsfejl. Fejl i digital OCES-signatur enten på id-kortet eller på hele kuverten");
                }
            }
            else
            {
                var unt = scd.Element(ns.xwsse + "UsernameToken");
                if (unt != null)
                {
                    if (unt.Element(ns.xwsse + "Username") == null || unt.Element(ns.xwsse + "Password") == null)
                    {
                        return new Tuple<string, string>("invalid_username_password", "Autentifikation eller autorisationsfejl. Fejl i username/password");
                    }
                }
            }

            return null;
        }

        public static Saml2Assertion MakeHealthContextAssertion(string nameIdentifier,
            string subjectName,
            string itSystem,
            string userAuthorizationCode = null,
            string userEducationCode = null,
            string userGivenName = null,
            string userSurName = null)
        {
            var sa = new Saml2Assertion(new Saml2NameIdentifier(nameIdentifier))
            {
                Subject = new Saml2Subject(new Saml2NameIdentifier(subjectName, new Uri(SamlValues.NameidFormatX509SubjectName)))
            };
            sa.Subject.SubjectConfirmations.Add(new Saml2SubjectConfirmation(new Uri(SamlValues.ConfirmationMethodSenderVouches)));
            var attributes = new[]
            {
                userAuthorizationCode == null
                    ? null
                    : new Saml2Attribute(HealthcareSamlAttributes.UserAuthorizationCode, userAuthorizationCode)
                    {
                        NameFormat = new Uri(SamlValues.NameFormatBasic)
                    },
                userEducationCode == null
                    ? null
                    : new Saml2Attribute(HealthcareSamlAttributes.UserEducationCode, userEducationCode)
                    {
                        NameFormat = new Uri(SamlValues.NameFormatBasic)
                    },
                userGivenName == null
                    ? null
                    : new Saml2Attribute(HealthcareSamlAttributes.UserGivenName, userGivenName)
                    {
                        NameFormat = new Uri(SamlValues.NameFormatBasic)
                    },
                userSurName == null
                    ? null
                    : new Saml2Attribute(HealthcareSamlAttributes.UserSurName, userSurName)
                    {
                        NameFormat = new Uri(SamlValues.NameFormatBasic)
                    },
                new Saml2Attribute(HealthcareSamlAttributes.ItSystemName, itSystem)
                {
                    NameFormat = new Uri(SamlValues.NameFormatBasic)
                }
            };
            sa.Statements.Add(new Saml2AttributeStatement(attributes.Where(a => a != null)));
            return sa;
        }


        public static SealCard SignIn(SealCard sc, string issuer, string endpointAdr)
        {
            var ss = WebPost(MakeDgwsStsReq(sc, issuer), endpointAdr);

            var fault = ss.Element(ns.xsoap + "Body").Element(ns.xsoap + "Fault");
            if (fault != null)
            {
                throw new FaultException(fault.Element("faultstring").Value, new FaultCode(fault.Element("faultcode").Value));
            }

            if (!new SealSignedXml(ss).CheckAssertionSignature())
            {
                throw new FaultException("Signature error", new FaultCode("STS"));
            }

            return new SealCard(ss.Descendants(ns.xsaml + "Assertion").First());
        }

        private static XElement MakeDgwsStsReq(SealCard sc, string issuer)
        {
            var xassertion = new XDocument();
            using (var wr = xassertion.CreateWriter())
            {
                sc.Xassertion.WriteTo(wr);
            }

            var xrst = new XElement(ns.xwst + "RequestSecurityToken",
                new XAttribute("Context", "www.sosi.dk"),
                new XElement(ns.xwst + "TokenType", "urn:oasis:names:tc:SAML:2.0:assertion"),
                new XElement(ns.xwst + "RequestType", "http://schemas.xmlsoap.org/ws/2005/02/security/trust/Issue"),
                new XElement(ns.xwst + "Claims", xassertion.Root),
                new XElement(ns.xwst + "Issuer",
                    new XElement(ns.xwsa04 + "Address", issuer)
                    )
                );

            return new XElement(ns.xsoap + "Envelope",
                new XElement(ns.xsoap + "Header",
                    new XElement(ns.xwsse + "Security",
                        new XElement(ns.xwsu + "Timestamp",
                            new XElement(ns.xwsu + "Created", DateTime.Now.ToString("u").Replace(' ', 'T'))
                            )
                        )
                    ),
                new XElement(ns.xsoap + "Body", xrst)
                );
        }


        private static XElement WebPost(XElement request, string url)
        {
            try
            {
                var WebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                WebRequest.Method = "POST";
                WebRequest.ContentType = "text/xml; charset=utf-8";
                WebRequest.Headers.Add("SOAPAction", "http://sosi.org/webservices/sts/1.0/stsService/RequestSecurityToken");
                using (var ms = new MemoryStream())
                {
                    var w = new XmlTextWriter(ms, Encoding.UTF8);
                    request.Save(w);
                    w.Flush();

                    WebRequest.ContentLength = ms.Length;
                    ms.Position = 0;
                    ms.CopyTo(WebRequest.GetRequestStream());
                }

                var response = WebRequest.GetResponse();
                return XElement.Load(response.GetResponseStream());
            }
            catch (WebException ex)
            {
                if (ex.Response == null) throw;
                return XElement.Load(ex.Response.GetResponseStream());
            }
        }

        internal static void SetSamlDsPreFix(XElement e)
        {
            var ass = e.DescendantsAndSelf(ns.xsaml + "Assertion").First();
            ass.Add(new XAttribute(XNamespace.Xmlns + "saml", ns.saml), new XAttribute(XNamespace.Xmlns + "ds", ns.ds));
            var q = from a in ass.DescendantsAndSelf().Attributes()
                where a.Name.LocalName == "xmlns" && string.IsNullOrEmpty(a.Name.NamespaceName)
                select a;
            foreach (var a in q)
            {
                a.Remove();
            }
        }

        internal static void CheckAndSetSamlDsPreFix(XDocument xdoc)
        {
            var signature = xdoc.Descendants(ns.xsaml + "Assertion").Elements(ns.xds + "Signature").FirstOrDefault();
            if (signature != null)
            {
                var ss = new SealSignedXml(xdoc);
                if (!ss.CheckAssertionSignature())
                {
                    SetSamlDsPreFix(xdoc.Root);
                    var ss2 = new SealSignedXml(xdoc.Root);
                    if (!ss2.CheckAssertionSignature())
                    {
                        throw new FaultException("Error in signature of assertion in requestheader");
                    }
                }
            }
        }
    }

    internal class faultwriter : XmlObjectSerializer
    {
        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            throw new NotImplementedException();
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            var xe = graph as string;
            writer.WriteElementString("FaultCode", ns.dgws, xe);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
        }
    }
}