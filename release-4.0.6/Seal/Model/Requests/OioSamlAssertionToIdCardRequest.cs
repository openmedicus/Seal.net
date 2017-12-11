using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.Constants;
using dk.nsi.seal.Model.Constants;
using dk.nsi.seal.Model.DomBuilders;

namespace dk.nsi.seal.Model.Requests
{
	public class OioSamlAssertionToIdCardRequest : OioWsTrustRequest
	{

		private Saml2Assertion contextToken;

		public string UserAuthorizationCode => contextToken.GetAttributeValue(HealthcareSamlAttributes.UserAuthorizationCode);
		public string UserEducationCode => contextToken.GetAttributeValue(HealthcareSamlAttributes.UserEducationCode);
		public string UserGivenName => contextToken.GetAttributeValue(HealthcareSamlAttributes.UserGivenName);
		public string UserSurName => contextToken.GetAttributeValue(HealthcareSamlAttributes.UserSurName);
		public string ItSystemName => contextToken.GetAttributeValue(HealthcareSamlAttributes.ItSystemName);

		public OioSamlAssertionToIdCardRequest(XDocument doc) : base(doc)
		{
			List<XElement> assertions = GetTags(new List<ITag>() { SoapTags.Envelope, SoapTags.Body, WstTags.RequestSecurityToken, Wst14Tags.ActAs, SamlTags.Assertion });
			if (assertions.Count > 1)
			{
				contextToken = new Saml2AssertionSerializer().ReadSaml2Assertion(assertions[1].CreateReader());
			}
		}

		public OioSamlAssertion OioSamlAssertion => new OioSamlAssertion(GetTag(new List<ITag>() { SoapTags.Envelope, SoapTags.Body, WstTags.RequestSecurityToken, Wst14Tags.ActAs, SamlTags.Assertion }));
	}

}
