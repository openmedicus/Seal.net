using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.Model;
using dk.nsi.seal.Model.ModelBuilders;
using dk.nsi.seal.pki;
using dk.nsi.seal.Vault;

namespace dk.nsi.seal.Factories
{
    public class SOSIFactory
    {
        public string PROPERTYNAME_SOSI_DGWS_VERSION { get { return "sosi:dgws.version"; } }
        public const string SOSI_DEFAULT_DGWS_VERSION = "1.0.1";

        public string PROPERTYNAME_SOSI_ISSUER { get { return "sosi:issuer"; } }
        public const string SOSI_DEFAULT_ISSUER = "TheSOSILibrary";

        public ISignatureProvider SignatureProvider { get; }
        public Federation.Federation Federation { get; }

        public SOSIFactory(Federation.Federation federation, ISignatureProvider signatureProvider)
        {
            Federation = federation;
            SignatureProvider = signatureProvider;
        }

        public ICredentialVault GetCredentialVault()
        {
	        var credentialVaultSignatureProvider = SignatureProvider as CredentialVaultSignatureProvider;
	        return credentialVaultSignatureProvider?.Vault;
        }

        public SystemIdCard CreateNewSystemIdCard(string itSystemName, CareProvider careProvider, AuthenticationLevel authenticationLevel, string username, string password,
            X509Certificate2 certificate, string alternativeIdentifier)
        {
            SystemInfo systemInfo = new SystemInfo(careProvider, itSystemName);
            return new SystemIdCard(GetDgwsVersion(), authenticationLevel, GetIssuer(), systemInfo, certificate?.GetCertHashString(), alternativeIdentifier, username, password);
        }

        public UserIdCard CreateNewUserIdCard(string itSystemName, UserInfo userInfo, CareProvider careProvider, AuthenticationLevel authenticationLevel, string username,
            string password, X509Certificate2 certificate, string alternativeIdentifier)
        {
            SystemInfo systemInfo = new SystemInfo(careProvider, itSystemName);
            return new UserIdCard(GetDgwsVersion(), authenticationLevel, GetIssuer(), systemInfo, userInfo, certificate?.GetCertHashString(), alternativeIdentifier, username, password);
        }

        public IdCard DeserializeIdCard<T>(T assertion)
        {
            IdCardModelBuilder builder = new IdCardModelBuilder();
            return builder.BuildModel(SerializerUtil.Serialize(assertion).Root);
        }

        private string GetDgwsVersion()
        {
            var version = SOSI_DEFAULT_DGWS_VERSION;
            if (ConfigurationManager.AppSettings.AllKeys.Contains(PROPERTYNAME_SOSI_DGWS_VERSION))
            {
                version = ConfigurationManager.AppSettings[PROPERTYNAME_SOSI_DGWS_VERSION];
            }
            return version;
        }

        private string GetIssuer()
        {
            var issuer = SOSI_DEFAULT_ISSUER;
            if (ConfigurationManager.AppSettings.AllKeys.Contains(PROPERTYNAME_SOSI_ISSUER))
            {
                issuer = ConfigurationManager.AppSettings[PROPERTYNAME_SOSI_ISSUER];
            }
            return issuer;
        }
    }
}
