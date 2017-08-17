using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dk.nsi.seal.Constants;
using dk.nsi.seal.Model.Constants;
using dk.nsi.seal.Vault;

namespace dk.nsi.seal.Model.DomBuilders
{
	public abstract class AbstractOioSamlTokenBuilder : AbstractSamlBuilder
	{
		public ICredentialVault SigningVault { get; set; }

		/// <summary>
		/// <b>Mandatory</b>: Set the audience restriction part of the message.<br />
		/// Example:
		///
		/// <pre>
		///  &lt;saml:Conditions... &gt;
		///      &lt;saml:AudienceRestriction&gt;http://fmk-online.dk&lt;/saml:AudienceRestriction&gt;
		///  &lt;/saml:Conditions&gt;
		/// </pre>
		/// </summary>
		public string AudienceRestriction { get; set; }

		/// <summary>
		/// <b>Mandatory</b>: Set the date/time when the oiosaml token is valid from.<br />
		/// Example:
		///
		/// <pre>
		///  &lt;saml:Conditions NotBefore = "2011-07-23T15:32:12Z"... & gt;
		///      ...
		///  &lt;/saml:Conditions&gt;
		/// </pre>
		/// </summary>
		public DateTime NotBefore { get; set; }

		/// <summary>
		/// <b>Mandatory</b>: Set the date/time when the oiosaml token expires.<br />
		/// Example:
		///
		/// <pre>
		///  &lt;saml:Conditions...NotOnOrAfter="2011-07-23T15:32:12Z"&gt;
		///      ...
		///  &lt;/saml:Conditions&gt;
		/// </pre>
		/// </summary>
		public DateTime NotOnOrAfter { get; set; }

		/// <summary>
		/// <b>Mandatory</b>: Set the UserIdCard from which attributes for the oiosaml token are extracted.
		/// </summary>
		public UserIdCard UserIdCard { get; set; }
		protected bool ExtractCprNumber { get; }
		protected bool ExtractCvrNumberIdentifier { get; }
		protected bool ExtractOrganizationName { get; }
		protected bool ExtractItSystemName { get; }
		protected bool ExtractUserAuthorizationCode { get; }
		protected bool ExtractUserEducationCode { get; }


	protected override void ValidateBeforeBuild()
		{
			Validate("UserIdCard", UserIdCard);
			Validate("NotBefore", NotBefore);
			Validate("AudienceRestriction", AudienceRestriction);
			Validate("NotOnOrAfter", NotOnOrAfter);
			Validate("SigningVault", SigningVault);

			if (NotBefore > NotOnOrAfter)
			{
				throw new ModelException("notBefore is after notOnOrAfter");
			}
		}

		protected override void AppendToRoot(XElement assertion)
		{
			base.AppendToRoot(assertion);
			CreateSubject(assertion);
			CreateConditions(assertion);
			CreateAuthnStatement(assertion);
			CreateAttributeStatement(assertion);
		}

		

		private void CreateAttributeStatement(XElement assertion)
		{
			var attributeStatement = XmlUtil.CreateElement(SamlTags.AttributeStatement);

			attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.Surname, OioSamlAttributes.SurnameFriendly, UserIdCard.UserInfo.SurName));
			attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.CommonName, OioSamlAttributes.CommonNameFriendly, UserIdCard.UserInfo.GivenName + " " + UserIdCard.UserInfo.SurName));
			attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.Email, OioSamlAttributes.EmailFriendly, UserIdCard.UserInfo.Email));
			attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.AssuranceLevel, null, SamlValues.Assurancelevel3));
			attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.SpecVersion, null, SamlValues.DkSpecVersion));
			if (ExtractCvrNumberIdentifier)
			{
				if (!SubjectIdentifierTypeValues.CvrNumber.Equals(UserIdCard.SystemInfo.CareProvider.Type))
				{
					throw new ArgumentException("CVR no. not provided in CareProvider - was " + UserIdCard.SystemInfo.CareProvider.Type);
				}
				attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.CvrNumber, null, UserIdCard.SystemInfo.CareProvider.Id));
			}
			if (ExtractOrganizationName)
			{
				attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.OrganizationName, OioSamlAttributes.OrganizationNameFriendly, UserIdCard.SystemInfo.CareProvider.OrgName));
			}
			if (ExtractCprNumber)
			{
				attributeStatement.Add(CreateAttributeElement(OioSamlAttributes.CprNumber, null, UserIdCard.UserInfo.Cpr));
			}
			if (ExtractUserAuthorizationCode)
			{
				if (UserIdCard.UserInfo.AuthorizationCode == null)
				{
					throw new ArgumentException("UserAuthorizationCode missing in UserIdCard - cannot create OIOSAML Token with UserAuthorizationCode");
				}
				attributeStatement.Add(CreateAttributeElement(HealthcareSamlAttributes.UserAuthorizationCode, null, UserIdCard.UserInfo.AuthorizationCode));
			}
			if (ExtractItSystemName)
			{
				attributeStatement.Add(CreateAttributeElement(HealthcareSamlAttributes.ItSystemName, null, UserIdCard.SystemInfo.ItSystemName));
			}
			if (ExtractUserEducationCode)
			{
				if (UserIdCard.UserInfo.AuthorizationCode == null)
				{
					throw new ArgumentException("UserAuthorizationCode must also be set on UserIdCard in order to treat contents of UserRole as UserEducationCode");
				}
				attributeStatement.Add(CreateAttributeElement(HealthcareSamlAttributes.UserEducationCode, null, UserIdCard.UserInfo.Role));
			}
			AddExtraAttributes(attributeStatement);
			assertion.Add(attributeStatement);
		}

		private void CreateAuthnStatement(XElement assertion)
		{
			var authnStatements = XmlUtil.CreateElement(SamlTags.AuthnStatement);
			authnStatements.Add(new XAttribute(SamlAttributes.AuthnInstant, UserIdCard.CreatedDate));
			var authnContext = XmlUtil.CreateElement(SamlTags.AuthnContext);

			var authnContextClassRef = XmlUtil.CreateElement(SamlTags.AuthnContextClassRef);
			authnContextClassRef.Value = SamlValues.AuthnContextClassRef;
			authnContext.Add(authnContextClassRef);
			authnStatements.Add(authnContext);

			assertion.Add(authnStatements);

		}

		private void CreateConditions(XElement assertion)
		{
			var conditions = XmlUtil.CreateElement(SamlTags.Conditions);
			conditions.Add(new XAttribute(SamlAttributes.NotBefore, NotBefore.FormatDateTimeXml()));
			conditions.Add(new XAttribute(SamlAttributes.NotOnOrAfter, NotOnOrAfter.FormatDateTimeXml()));

			var audienceRestriction = XmlUtil.CreateElement(SamlTags.AudienceRestriction);
			var audience = XmlUtil.CreateElement(SamlTags.Audience);
			audience.Value = AudienceRestriction;
			audienceRestriction.Add(audience);
			conditions.Add(audienceRestriction);

			assertion.Add(conditions);
		}

		protected void SignAssertion(XDocument document)
		{
			//SignatureConfiguration signatureConfiguration = new SignatureConfiguration(new string[] { assertionID }, assertionID, IDValues.Id);
			//signatureConfiguration.setSignatureSiblingNode(subjectNode);
			//AddExtraSignatureConfiguration(signatureConfiguration);

			//SignatureUtil.Sign(SignatureProviderFactory.fromCredentialVault(signingVault), document, signatureConfiguration);
			var signer = new SealSignedXml(document);
			XmlDocument envelope = signer.SignAssertion(SigningVault.GetSystemCredentials(), AssertionId);
			// TODO: Assign back
		}


		protected abstract void AddExtraAttributes(XElement assertion);

		protected abstract void AddExtraAuthnAttributes(XElement assertion);

	}
}
