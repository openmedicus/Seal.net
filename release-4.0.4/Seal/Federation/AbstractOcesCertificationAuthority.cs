using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
    public abstract class AbstractOcesCertificationAuthority : ICertificationAuthority
    {
        private readonly ICertificateStatusChecker CertificateStatusChecker;
        public AbstractOcesCertificationAuthority(ICertificateStatusChecker certificateStatusChecker)
        {
            if (certificateStatusChecker == null) throw new ArgumentException("'certificateStatusChecker' cannot be null");
            CertificateStatusChecker = certificateStatusChecker;
        }
        protected abstract X509Certificate2 GetOCES2RootCertificate();

        protected abstract string GetCertificationAuthorityName();

        public bool IsValid(X509Certificate2 certificate)
        {
            return GetCertificateStatus(certificate).IsValid;
        }

        public CertificateStatus GetCertificateStatus(X509Certificate2 certificate)
        {
            CertificateStatus certificateStatus;
            if (CheckDates(certificate))
            {
                //Compare with root certificate
                if (!CompareWithRoot(certificate))
                {
                    throw new CryptographicException("The supplied certificate with DN '" + new DistinguishedName(certificate.Subject) + "' is not a " + GetCertificationAuthorityName() + " certificate");
                }

                certificateStatus = CheckRevocation(certificate);
            }
            else
            {
                certificateStatus = new CertificateStatus(false, null);
            }
            return certificateStatus;
        }

        private bool CheckDates(X509Certificate2 certificate)
        {
            if (certificate.NotAfter < DateTime.Now)
            {
                return false; // Certificate is expired
            }
            else if (certificate.NotBefore > DateTime.Now)
            {
                return false; // Certificate is not yet valid
            }
            return true;
        }

        private bool CompareWithRoot(X509Certificate2 certificateToValidate)
        {
            X509Certificate2 authority = GetOCES2RootCertificate();

            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.ExtraStore.Add(authority);

            chain.Build(certificateToValidate);

            X509Certificate2 chainRoot = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
            return chainRoot.Equals(authority);
        }

        private CertificateStatus CheckRevocation(X509Certificate2 certificate)
        {
            try
            {
                return CertificateStatusChecker.GetRevocationStatus(certificate);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
