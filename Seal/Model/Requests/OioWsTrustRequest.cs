using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using dk.nsi.seal.Model.Constants;
using dk.nsi.seal.Model.ModelBuilders;
using dk.nsi.seal.Vault;
using System.Configuration;

namespace dk.nsi.seal.Model.Requests
{
    public class OioWsTrustRequest : OioWsTrustMessage
    {
        public OioWsTrustRequest(XDocument doc) : base(doc)
        {
        }

        public string AppliesTo => SafeGetTagTextContent(new List<ITag>() { SoapTags.Envelope, SoapTags.Body, WstTags.RequestSecurityToken, WspTags.AppliesTo, WsaTags.EndpointReference, WsaTags.Address });

        public string Context
        {
            get
            {
                var ac = GetTag(new List<ITag>() { SoapTags.Envelope, SoapTags.Body, WstTags.RequestSecurityToken });
                return ac.Attributes(WsTrustAttributes.Context).FirstOrDefault()?.Value;
            }
        }


		/// <summary>
		/// Checks the signature on the <see cref="OioWsTrustRequest"/> and whether the signing certificate is trusted.
		/// </summary>
		/// <param name="vault">The CredentialVault containing trusted certificates used to check trust for the <see cref="OioWsTrustRequest"/>.</param>
		public void ValidateSignatureAndTrust(ICredentialVault vault)
        {
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

			if (!SignatureUtil.Validate(dom, null, vault, checkTrust, checkCrl))
            {
                throw new ModelBuildException("Liberty signature could not be validated");
            }
        }
    }
}
