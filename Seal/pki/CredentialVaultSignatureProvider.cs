using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using dk.nsi.seal.dgwstypes;
using dk.nsi.seal.Model;
using dk.nsi.seal.Vault;

namespace dk.nsi.seal.pki
{
    public class CredentialVaultSignatureProvider : ISignatureProvider
    {
        public ICredentialVault Vault { get; }

        public CredentialVaultSignatureProvider(ICredentialVault vault)
        {
            if (vault == null)
            {
                throw new ArgumentException("CredentialVault cannot be null");
            }
            Vault = vault;
        }

        public XElement Sign(Assertion ass)
        {
            ass = SealUtilities.SignAssertion(ass, Vault.GetSystemCredentials());
            return SerializerUtil.Serialize(ass).Root;
        }
    }
}
