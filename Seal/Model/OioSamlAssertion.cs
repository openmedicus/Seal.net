using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using dk.nsi.seal.Model.Constants;
using dk.nsi.seal.Model.ModelBuilders;
using dk.nsi.seal.Vault;
using System.Configuration;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace dk.nsi.seal.Model
{
	public class OioSamlAssertion : AbstractDomInfoExtractor
	{
		public string Id => dom.Attribute(SamlAttributes.Id).Value;
		public string Issuer => dom.Descendants(SamlTags.Issuer.Ns + SamlTags.Issuer.TagName).FirstOrDefault()?.Value;

		public string CommonName => GetAttributeValue(OioSamlAttributes.CommonName);
		public string Cpr => GetAttributeValue(OioSamlAttributes.CprNumber);
		public string CvrNumberIdentifier => GetAttributeValue(OioSamlAttributes.CvrNumber);
		public string Email => GetAttributeValue(OioSamlAttributes.Email);
		public DateTime NotBefore => DateTime.Parse(dom.Descendants(SamlTags.Conditions.Ns + SamlTags.Conditions.TagName).FirstOrDefault()?.Attribute(SamlAttributes.NotBefore)?.Value);
		public DateTime NotOnOrAfter => DateTime.Parse(dom.Descendants(SamlTags.Conditions.Ns + SamlTags.Conditions.TagName).FirstOrDefault()?.Attribute(SamlAttributes.NotOnOrAfter)?.Value);
		public string OrganizationName => GetAttributeValue(OioSamlAttributes.OrganizationName);
		public string SurName => GetAttributeValue(OioSamlAttributes.Surname);
		public string AssuranceLevel => GetAttributeValue(OioSamlAttributes.AssuranceLevel);
		public string SpecVersion => GetAttributeValue(OioSamlAttributes.SpecVersion);
		public string AudienceRestriction => dom.Descendants(SamlTags.Audience.Ns + SamlTags.Audience.TagName).FirstOrDefault()?.Value;
		public DateTime UserAuthenticationInstant => DateTime.Parse(dom.Descendants(SamlTags.AuthnStatement.Ns + SamlTags.AuthnStatement.TagName).FirstOrDefault()?.Attribute(SamlAttributes.AuthnInstant)?.Value);

		public string RidNumberIdentifier => GetAttributeValue(OioSamlAttributes.RidNumber);
		public string CertificateIssuer => GetAttributeValue(OioSamlAttributes.CertificateIssuer);
		public string CertificateSerial => GetAttributeValue(OioSamlAttributes.CertificateSerial);

		public string Recipient
			=>
				dom.Descendants(SamlTags.SubjectConfirmationData.Ns + SamlTags.SubjectConfirmationData.TagName)
					.FirstOrDefault()?.Attribute(SamlAttributes.Recipient)?.Value;
		public string SubjectNameId => dom.Descendants(SamlTags.NameID.Ns + SamlTags.NameID.TagName).FirstOrDefault()?.Value;

		public string SubjectNameIdFormat
			=>
				dom.Descendants(SamlTags.NameID.Ns + SamlTags.NameID.TagName)
					.FirstOrDefault()?.Attribute(SamlAttributes.Format)?.Value;
		public bool IsYouthCertificate => bool.Parse(GetAttributeValue(OioSamlAttributes.IsYouthCert));
		public string Uid => GetAttributeValue(OioSamlAttributes.Uid);

		public X509Certificate2 UserCertificate => new X509Certificate2(Convert.FromBase64String(GetAttributeValue(OioSamlAttributes.UserCertificate)));



		public OioSamlAssertion(XElement assertionElement) : base(assertionElement)
		{
			ValidateElement(dom);
		}

		public OioSamlAssertion(Saml2Assertion assertion) : base(Saml2AssertionToXElement(assertion))
		{

			ValidateElement(dom);
		}

		private static XElement Saml2AssertionToXElement(Saml2Assertion assertion)
		{
			var sb = new StringBuilder();
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Encoding = Encoding.UTF8
			};
			using (var stringWriter = new StringWriter(sb))
			using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
			using (var dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter))
			{
				new Saml2AssertionSerializer().WriteSaml2Assertion(xmlWriter, assertion);
			}
			return XElement.Parse(sb.ToString());
		}

		private void ValidateElement(XElement element)
		{
			if (!(NameSpaces.xsaml.Equals(element.Name.Namespace)
					&& element.Name.Equals(SamlTags.Assertion.Ns + SamlTags.Assertion.TagName)))
			{
				throw new ArgumentException("Element is not a SAML assertion");
			}
		}

		public UserIdCard UserIdCard
		{
			get
			{
				//var attributes = dom.Descendants(SamlTags.Attribute.Ns + SamlTags.Attribute.TagName);
				//var discoveryEprNode = attributes.FirstOrDefault(element => element.Attribute(SamlAttributes.Name).Value.Equals(OioSamlAttributes.DiscoveryEpr));
				var metadata = dom.Descendants(WsaTags.Metadata.Ns + WsaTags.Metadata.TagName).FirstOrDefault();
				var serviceType = dom.Descendants(LibertyDiscoveryTags.ServiceType.Ns + LibertyDiscoveryTags.ServiceType.TagName).FirstOrDefault();
				if (serviceType != null && LibertyValues.SosiUrn.Equals(serviceType.Value))
				{
					var idCardElm = GetTag(metadata, new List<ITag>() { WsaTags.Metadata, LibertyDiscoveryTags.SecurityContext, LibertySecurityTags.Token, SamlTags.Assertion});
					return (UserIdCard)new IdCardModelBuilder().BuildModel(idCardElm);
				}
				else
				{
					return null;
				}
			}
		}


		/// <summary>
		/// Checks the signature on the <see cref="OioSamlAssertion"/>.
		/// </summary>
		/// <param name="vault">The <see cref="ICredentialVault"/> containing trusted certificates used to check trust for the <see cref="OioSamlAssertion"/>.</param>
		public void ValidateSignatureAndTrust(ICredentialVault vault)
		{
			
			var signatureElement = dom.Element(DsTags.Signature.Ns + DsTags.Signature.TagName);//dom.XPathSelectElement("/" + );
			if (signatureElement == null)
			{
				throw new ModelException("OIOSAMLAssertion is not signed");
			}
			List<XElement> referencedSignedElements = SignatureUtil.DereferenceSignedElements(signatureElement, dom);
			if (!referencedSignedElements.Contains(dom))
			{
				throw new ModelException("OIOSAMLAssertion element is not referenced by contained signature");
			}
			var checkTrust = false;
			if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckTrust"))
			{
				checkTrust = ConfigurationManager.AppSettings["CheckTrust"].ToLower().Equals("true");
			}
			var checkCrl = false;
			if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckCrl"))
			{
				checkCrl = ConfigurationManager.AppSettings["CheckCrl"].ToLower().Equals("true");
			}

			if (!SignatureUtil.Validate(dom,null, vault, checkTrust, checkCrl))
			{
				throw new ModelException("Signature on OIOSAMLAssertion is invalid");
			}
		}

		public void Sign(ICredentialVault signingVault)
		{
			var signer = new SealSignedXml(XAssertion);
			var signedXml = signer.SignAssertion(signingVault.GetSystemCredentials(), XAssertion.Attribute(SamlAttributes.Id).Value);
			dom = XElement.Parse(signedXml.OuterXml, LoadOptions.PreserveWhitespace);
		}


		public string GetAttributeValue(string name)
		{
			var attributes = dom.Descendants(SamlTags.Attribute.Ns + SamlTags.Attribute.TagName);
			return attributes.FirstOrDefault(element =>
			{
				var xAttribute = element.Attribute(SamlAttributes.Name);
				return xAttribute != null && xAttribute.Value.Equals(name);
			})?.Value;
		}

		public void ValidateTimestamp()
		{
			ValidateTimestamp(0);
		}

		public void ValidateTimestamp(long allowedDriftInSeconds)
		{
			if (allowedDriftInSeconds < 0) throw new ArgumentException("'allowedDriftInSeconds' must not be negative!");
			var now = DateTimeEx.UtcNowRound;

			if (now.AddSeconds(allowedDriftInSeconds) < NotBefore)
			{
				throw new ModelException("OIOSAML token is not valid yet - now: " + now.FormatDateTimeXml() +
						". OIOSAML token validity start: " + NotBefore.FormatDateTimeXml() + ". Allowed clock drift: " + allowedDriftInSeconds + " seconds");
			}
			if (now.AddSeconds(-allowedDriftInSeconds) > NotOnOrAfter)
			{
				throw new ModelException("OIOSAML token no longer valid - now: " + now.FormatDateTimeXml() +
						". OIOSAML token validity end: " + NotOnOrAfter.FormatDateTimeXml() + ". Allowed clock drift: " + allowedDriftInSeconds + " seconds");
			}
		}

	}
}
