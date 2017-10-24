using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using dk.nsi.seal.Constants;
using dk.nsi.seal.Model.Constants;

namespace SealTest.AssertionTests.AssertionBuilders
{
    public class NemIdAssertionBuilder

    {
        private static readonly Uri BasicNameFormat = new Uri("urn:oasis:names:tc:SAML:2.0:attrname-format:basic");
        private static string _specVersion = "DK-SAML-2.0";

        public static Saml2Assertion MakeNemIdAssertion(
            X509Certificate2 userCertificate,
            X509Certificate2 signingCertificate,
            string userCpr,
            string userGivenName,
            string userSurName,
            string userEmail,
            string userRole, string assuranceLevel, string cvrNumber, string organizationName,
            string userAuthorizationCode = null
            )
        {
            var ass = new Saml2Assertion(new Saml2NameIdentifier("https://saml.test-nemlog-in.dk/"))
            {
                Conditions = new Saml2Conditions
                {
                    NotOnOrAfter = DateTime.Now + TimeSpan.FromHours(8),
                    NotBefore = DateTime.Now
                },
                Subject = new Saml2Subject(new Saml2NameIdentifier(userCertificate.SubjectName.Name))
            };

            ass.Subject.SubjectConfirmations.Add(
                new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"))
                {
                    SubjectConfirmationData = new Saml2SubjectConfirmationData
                    {
                        NotOnOrAfter = DateTime.Now + TimeSpan.FromHours(8),
                        Recipient = new Uri("https://staging.fmk-online.dk/fmk/saml/SAMLAssertionConsumer")
                    }
                });

            IList<Saml2Attribute> q = new List<Saml2Attribute>();

            q.Add(new Saml2Attribute(OioSamlAttributes.SpecVersion, _specVersion) { NameFormat = BasicNameFormat });

            // User
            q.Add(new Saml2Attribute(OioSamlAttributes.CommonName, userGivenName) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.Surname, userSurName) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.Email, userEmail) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.CprNumber, userCpr) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.AssuranceLevel, assuranceLevel) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.UserCertificate, Convert.ToBase64String(userCertificate.RawData)) { NameFormat = BasicNameFormat });

            // Organization
            q.Add(new Saml2Attribute(OioSamlAttributes.CvrNumber, cvrNumber) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.OrganizationName, organizationName) { NameFormat = BasicNameFormat });

            // Certificate
            var subjectSerialNumber = userCertificate.SubjectName.Name;
            q.Add(new Saml2Attribute(OioSamlAttributes.CertificateSerial, userCertificate.GetSerialNumberString()) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.CertificateIssuer, userCertificate.IssuerName.Name) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.Uid, ExtractUidNumber(subjectSerialNumber)) { NameFormat = BasicNameFormat });
            q.Add(new Saml2Attribute(OioSamlAttributes.RidNumber, ExtractRidNumber(subjectSerialNumber)) { NameFormat = BasicNameFormat });

            ass.Statements.Add(new Saml2AttributeStatement(q));
            ass.Statements.Add(
                new Saml2AuthenticationStatement(
                    new Saml2AuthenticationContext(new Uri("element:urn:oasis:names:tc:SAML:2.0:ac:classes:X509")),
                    DateTime.Now));

            ass.SigningCredentials = new X509SigningCredentials(signingCertificate, SignedXml.XmlDsigRSASHA1Url,
                SignedXml.XmlDsigSHA1Url);

            return ass;
        }

        private static string ExtractRidNumber(string subjectSerialNumber)
        {
            const string ridIdentifier = "RID:";
            var index = subjectSerialNumber.IndexOf(ridIdentifier, StringComparison.CurrentCultureIgnoreCase);
            if (index == -1)
            {
                throw new Exception("Could not extract RID number from subject serial number: '" + subjectSerialNumber +
                                    "'");
            }
            return subjectSerialNumber.Substring(index + ridIdentifier.Length).Split(' ').First();
        }

        private static string ExtractUidNumber(string subjectSerialNumber)
        {
            const string cvrIdentifier = "CVR:";
            var index = subjectSerialNumber.IndexOf(cvrIdentifier, StringComparison.CurrentCultureIgnoreCase);
            if (index == -1)
            {
                throw new Exception("Could not extract RID number from subject serial number: '" + subjectSerialNumber +
                                    "'");
            }
            return subjectSerialNumber.Substring(index).Split(' ').First();
        }
    }
}
