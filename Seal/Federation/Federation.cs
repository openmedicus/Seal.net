using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dk.nsi.seal.Federation
{
	public abstract class Federation
	{
	    public ICertificationAuthority CertificationAuthority { get; }

        //private final AuditEventHandler eventHandler;
        //   private final Properties properties;

        /**
         * Construct an instance of <code>Federation</code>
         *
         * @param certificationAuthority the CA used in the federation
         */
        protected Federation(ICertificationAuthority certificationAuthority)
        {
            CertificationAuthority = certificationAuthority;
        }

        /**
         * Returns <code>true</code> if the passed certificate is a valid certificate issued to the STS of the federation and <code>false</code>
         * otherwise.
         * 
         * @param certificate
         *            the certificate to check.
         */
        public bool IsValidSTSCertificate(X509Certificate2 certificate)
        {
            if (!SubjectDistinguishedNameMatches(new DistinguishedName(certificate.Subject)))
            {
                return false; // NOPMD
            }

            return CertificationAuthority.IsValid(certificate);
        }

        /**
         * Returns <code>true</code> if the passed subjectDistinguishedName matches an STS of the federation and <code>false</code>
         * otherwise.
         *
         * @param subjectDistinguishedName
         *            the subjectDistinguishedName to check.
         */
        protected abstract bool SubjectDistinguishedNameMatches(DistinguishedName subjectDistinguishedName);

        /**
         * Returns <code>true</code> if the passed certificate is a valid certificate issued by the CA of the federation and <code>false</code>
         * otherwise.
         * 
         * @param certificate
         *            the certificate to check.
         */
        public bool IsValidCertificate(X509Certificate2 certificate)
        {
            return CertificationAuthority.IsValid(certificate);
        }

        /**
         * Returns a combined result containing the result of a corresponding <link>isValidCertificate</link> call and the timestamp
         * for the revocation check involved.
         *
         * @param certificate to be checked.
         * @return the combined result.
         */
        public CertificateStatus GetCertificateStatus(X509Certificate2 certificate)
        {
            return CertificationAuthority.GetCertificateStatus(certificate);
        }
    }
}
