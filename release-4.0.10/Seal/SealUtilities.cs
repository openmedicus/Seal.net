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
using dk.nsi.seal.Model.ModelBuilders;
using dk.nsi.seal.Model.Requests;
using dk.nsi.seal.Vault;

namespace dk.nsi.seal
{
    public class SealUtilities
    {
        public static T SignAssertion<T>(T element, X509Certificate2 cert) where T : class
        {
            var sxml = new SealSignedXml(SerializerUtil.Serialize(element));
            var xassertion = sxml.xml.GetElementsByTagName("Assertion", NameSpaces.saml)[0] as XmlElement;
            if (xassertion == null) throw new InvalidOperationException("Assertion not found");
            var keyName = xassertion.GetElementsByTagName("KeyName", NameSpaces.ds)[0].InnerText;

            var xsignature = sxml.GetDGWSSign(cert);
            xsignature.SetAttribute("id", keyName);
            xassertion.AppendChild(xsignature);

            return SerializerUtil.Deserialize<T>(sxml.xml.DocumentElement);
        }

        public static bool CheckAssertionSignature<T>(T element)
        {
            var ss = new SealSignedXml(SerializerUtil.Serialize(element));
            return ss.CheckAssertionSignature();
        }

        public static bool CheckAssertionSignature(XElement element)
        {
            var ss = new SealSignedXml(element);
            return ss.CheckAssertionSignature();
        }

        public static bool CheckAssertionSignatureNSCheck<T>(T element)
        {
            return CheckAssertionSignatureNSCheck(SerializerUtil.Serialize(element).Root);
        }

        public static bool CheckAssertionSignatureNSCheck(XElement element)
        {
            var ss = new SealSignedXml(element);
            if (ss.CheckAssertionSignature()) return true;
            SetSamlDsPreFix(element);
            ss = new SealSignedXml(element);
            return ss.CheckAssertionSignature();
        }

        public static X509Certificate2 GetAssertionSignature(XElement element)
        {
            var ss = new SealSignedXml(element);
            if (ss.CheckAssertionSignature()) return ss.GetSignature();
            SetSamlDsPreFix(element);
            ss = new SealSignedXml(element);
            return ss.GetSignature();
        }


        internal static FaultException MakeFault(string reason, string detail)
        {
            return new FaultException(MessageFault.CreateFault(new FaultCode("Server"), new FaultReason(reason), detail, new faultwriter()));
        }

        public static FaultException ValidateSecurity<T>(T security) where T : class
        {
            var t = ValidateSecurity(SerializerUtil.Serialize(security).Root);
            return MakeFault(t.Item1, t.Item1);
        }

        public static Tuple<string, string> ValidateSecurity(XElement security)
        {
            if (security == null || (security.Name != NameSpaces.xwsse + "Security")) return new Tuple<string, string>("invalid_idcard", "Security element mangler");
            var err = t.check(security, t.sectree);
            if (err != null) return err;

            var scd = security.Descendants(NameSpaces.xsaml + "SubjectConfirmationData").First();
            var ki = scd.Element(NameSpaces.xds + "KeyInfo");
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
                var unt = scd.Element(NameSpaces.xwsse + "UsernameToken");
                if (unt != null)
                {
                    if (unt.Element(NameSpaces.xwsse + "Username") == null || unt.Element(NameSpaces.xwsse + "Password") == null)
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

        [Obsolete("Deprecated, please use IdCard version")]
        public static SealCard SignIn(SealCard sc, string issuer, string endpointAdr)
        {
            var ss = WebPost(MakeDgwsStsReq(sc, issuer), endpointAdr);

            var fault = ss.Element(NameSpaces.xsoap + "Body").Element(NameSpaces.xsoap + "Fault");
            if (fault != null)
            {
                throw new FaultException(fault.Element("faultstring").Value, new FaultCode(fault.Element("faultcode").Value));
            }

            if (!new SealSignedXml(ss).CheckAssertionSignature())
            {
                throw new FaultException("Signature error", new FaultCode("STS"));
            }

            return new SealCard(ss.Descendants(NameSpaces.xsaml + "Assertion").First());
        }

        private static XElement MakeDgwsStsReq(SealCard sc, string issuer)
        {
            var xassertion = new XDocument();
            using (var wr = xassertion.CreateWriter())
            {
                sc.Xassertion.WriteTo(wr);
            }

            var xrst = new XElement(NameSpaces.xwst + "RequestSecurityToken",
                new XAttribute("Context", "www.sosi.dk"),
                new XElement(NameSpaces.xwst + "TokenType", "urn:oasis:names:tc:SAML:2.0:assertion"),
                new XElement(NameSpaces.xwst + "RequestType", "http://schemas.xmlsoap.org/ws/2005/02/security/trust/Issue"),
                new XElement(NameSpaces.xwst + "Claims", xassertion.Root),
                new XElement(NameSpaces.xwst + "Issuer",
                    new XElement(NameSpaces.xwsa04 + "Address", issuer)
                    )
                );

            return new XElement(NameSpaces.xsoap + "Envelope",
                new XElement(NameSpaces.xsoap + "Header",
                    new XElement(NameSpaces.xwsse + "Security",
                        new XElement(NameSpaces.xwsu + "Timestamp",
                            new XElement(NameSpaces.xwsu + "Created", DateTime.Now.ToString("u").Replace(' ', 'T'))
                            )
                        )
                    ),
                new XElement(NameSpaces.xsoap + "Body", xrst)
                );
        }

        public static IdCard SignIn(IdCard sc, string issuer, string endpointAdr)
        {
            var ss = WebPost(MakeDgwsStsReq(sc, issuer), endpointAdr);

            var fault = ss.Element(NameSpaces.xsoap + "Body").Element(NameSpaces.xsoap + "Fault");
            if (fault != null)
            {
                throw new FaultException(fault.Element("faultstring").Value, new FaultCode(fault.Element("faultcode").Value));
            }

            if (!new SealSignedXml(ss).CheckAssertionSignature())
            {
                throw new FaultException("Signature error", new FaultCode("STS"));
            }

            var builder = new IdCardModelBuilder();
            return builder.BuildModel(ss.Descendants(NameSpaces.xsaml + "Assertion").First());
        }

		public static IdCard SignIn(XElement request, string endpointAdr)
		{
			var ss = WebPost(request, endpointAdr);

			var fault = ss.Element(NameSpaces.xsoap + "Body").Element(NameSpaces.xsoap + "Fault");
			if (fault != null)
			{
				throw new FaultException(fault.Element("faultstring").Value, new FaultCode(fault.Element("faultcode").Value));
			}

			if (!new SealSignedXml(ss).CheckAssertionSignature())
			{
				throw new FaultException("Signature error", new FaultCode("STS"));
			}
			var idCardModelBuilder = new IdCardModelBuilder();
			return idCardModelBuilder.BuildModel(ss.Descendants(NameSpaces.xsaml + "Assertion").First());
		}

		public static IdCard SignIn(OioWsTrustRequest request, string endpointAdr)
		{
			var ss = WebPost(request.XAssertion, endpointAdr);

			var fault = ss.Element(NameSpaces.xsoap + "Body").Element(NameSpaces.xsoap + "Fault");
			if (fault != null)
			{
				throw new FaultException(fault.Element("faultstring").Value, new FaultCode(fault.Element("faultcode").Value));
			}
			var signed = new SealSignedXml(ss);

			if (!signed.CheckEnvelopeSignature())
			{
				throw new FaultException("Envelope Signature error", new FaultCode("STS"));
			}
			var idCardModelBuilder = new IdCardModelBuilder();
			return idCardModelBuilder.BuildModel(ss.Descendants(NameSpaces.xsaml + "Assertion").First());
		}

		private static XElement MakeDgwsStsReq(IdCard sc, string issuer)
        {
            var xassertion = new XDocument();
            using (var wr = xassertion.CreateWriter())
            {
                sc.Xassertion.WriteTo(wr);
            }

            var xrst = new XElement(NameSpaces.xwst + "RequestSecurityToken",
                new XAttribute("Context", "www.sosi.dk"),
                new XElement(NameSpaces.xwst + "TokenType", "urn:oasis:names:tc:SAML:2.0:assertion"),
                new XElement(NameSpaces.xwst + "RequestType", "http://schemas.xmlsoap.org/ws/2005/02/security/trust/Issue"),
                new XElement(NameSpaces.xwst + "Claims", xassertion.Root),
                new XElement(NameSpaces.xwst + "Issuer",
                    new XElement(NameSpaces.xwsa04 + "Address", issuer)
                    )
                );

            return new XElement(NameSpaces.xsoap + "Envelope",
                new XElement(NameSpaces.xsoap + "Header",
                    new XElement(NameSpaces.xwsse + "Security",
                        new XElement(NameSpaces.xwsu + "Timestamp",
                            new XElement(NameSpaces.xwsu + "Created", DateTime.Now.ToString("u").Replace(' ', 'T'))
                            )
                        )
                    ),
                new XElement(NameSpaces.xsoap + "Body", xrst)
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
            var ass = e.DescendantsAndSelf(NameSpaces.xsaml + "Assertion").First();
            ass.Add(new XAttribute(XNamespace.Xmlns + "saml", NameSpaces.saml), new XAttribute(XNamespace.Xmlns + "ds", NameSpaces.ds));
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
            var signature = xdoc.Descendants(NameSpaces.xsaml + "Assertion").Elements(NameSpaces.xds + "Signature").FirstOrDefault();
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
}