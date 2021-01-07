using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.pki
{
    public class SignatureResult
    {
        public string Signature { get; }
        public X509Certificate2 Certificate { get; }

        public SignatureResult(string signature, X509Certificate2 cert)
        {
            Signature = signature;
            Certificate = cert;
        }
    }
}
