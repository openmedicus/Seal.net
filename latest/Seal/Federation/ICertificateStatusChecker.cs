using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
    public interface ICertificateStatusChecker
    {
        CertificateStatus GetRevocationStatus(X509Certificate2 certificate);
    }
}
