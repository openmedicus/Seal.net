using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using dk.nsi.fmk;
using dk.nsi.seal;
using dk.nsi.seal.Constants;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Attribute = dk.nsi.fmk.Attribute;
using KeyInfo = dk.nsi.fmk.KeyInfo;

namespace SealTest
{
    public class AssertionMaker
    {
        public static Saml2Assertion MakeNemIdAssertion(X509Certificate2 certificate)
        {
            var doc = Global.SignedTokenXml();
            Saml2Assertion sa;
            using (var rd = doc.CreateReader())
            {
                var s2sth = new Saml2SecurityTokenHandler
                {
                    /*Configuration = new SecurityTokenHandlerConfiguration
                    {
                        IssuerTokenResolver = new Saml2IssuerTokenResolver()
                    }*/
                };
                var s2st = s2sth.ReadToken(rd) as Saml2SecurityToken;
                sa = s2st.Assertion;
            }

            var ass = new Saml2Assertion(new Saml2NameIdentifier(sa.Issuer.Value))
            {
                Conditions = new Saml2Conditions
                {
                    NotOnOrAfter = DateTime.Now + TimeSpan.FromHours(8),
                    NotBefore = DateTime.Now
                },
                Subject = new Saml2Subject(new Saml2NameIdentifier(certificate.SubjectName.Name))
            };

            ass.Subject.SubjectConfirmations.Add(new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer"))
            {
                SubjectConfirmationData = new Saml2SubjectConfirmationData
                {
                    NotOnOrAfter = DateTime.Now + TimeSpan.FromHours(8),
                    Recipient = new Uri("https://staging.fmk-online.dk/fmk/saml/SAMLAssertionConsumer")
                }
            });

            var q = from att in sa.Statements.OfType<Saml2AttributeStatement>().First().Attributes
                select new Saml2Attribute(att.Name, att.Values.First()) {NameFormat = att.NameFormat};

            ass.Statements.Add(new Saml2AttributeStatement(q));
            ass.Statements.Add(new Saml2AuthenticationStatement(new Saml2AuthenticationContext(new Uri("element:urn:oasis:names:tc:SAML:2.0:ac:classes:X509")), DateTime.Now));

            /*var secClause = new X509RawDataKeyIdentifierClause(certificate);
            var issuerKeyIdentifier = new SecurityKeyIdentifier(secClause);
            issuerKeyIdentifier.Add(secClause);*/

            ass.SigningCredentials = new X509SigningCredentials(certificate); //, SignedXml.XmlDsigRSASHA1Url, SignedXml.XmlDsigSHA1Url);
            return ass;
        }

        public static Assertion MakeAssertionForSTS(X509Certificate2 certificate)
        {
            var vnow = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5);

            var ass = new Assertion
            {
                IssueInstant = vnow,
                id = "IDCard",
                Version = 2.0m,
                Issuer = "WinPLC",
                Conditions = new Conditions
                {
                    NotBefore = vnow,
                    NotOnOrAfter = vnow + TimeSpan.FromHours(8)
                },
                Subject = new Subject
                {
                    NameID = new NameID
                    {
                        Format = "http://rep.oio.dk/cpr.dk/xml/schemas/core/2005/03/18/CPR_PersonCivilRegistrationIdentifier.xsd",
                        Value = "2203333571"
                    },
                    SubjectConfirmation = new SubjectConfirmation
                    {
                        ConfirmationMethod = ConfirmationMethod.urnoasisnamestcSAML20cmholderofkey,
                        SubjectConfirmationData = new SubjectConfirmationData
                        {
                            Item = new KeyInfo
                            {
                                Item = "OCESSignature"
                            }
                        }
                    }
                },
                AttributeStatement = new[]
                {
                    new AttributeStatement
                    {
                        id = AttributeStatementID.IDCardData,
                        Attribute = new[]
                        {
                            new Attribute {Name = SosiAttributes.IDCardID, AttributeValue = Guid.NewGuid().ToString("D")},
                            new Attribute {Name = SosiAttributes.IDCardVersion, AttributeValue = "1.0.1"},
                            new Attribute {Name = SosiAttributes.IDCardType, AttributeValue = "user"},
                            new Attribute {Name = SosiAttributes.AuthenticationLevel, AttributeValue = "4"}
                        }
                    },
                    new AttributeStatement
                    {
                        id = AttributeStatementID.UserLog,
                        Attribute = new[]
                        {
                            new Attribute {Name = MedComAttributes.UserCivilRegistrationNumber, AttributeValue = "1802602810"},
                            new Attribute {Name = MedComAttributes.UserGivenName, AttributeValue = "Stine"},
                            new Attribute {Name = MedComAttributes.UserSurname, AttributeValue = "Svendsen"},
                            new Attribute {Name = MedComAttributes.UserEmailAddress, AttributeValue = "stineSvendsen@example.com"},
                            new Attribute {Name = MedComAttributes.UserRole, AttributeValue = "7170"},
                            new Attribute {Name = MedComAttributes.UserAuthorizationCode, AttributeValue = "ZXCVB"}
                        }
                    },
                    new AttributeStatement
                    {
                        id = AttributeStatementID.SystemLog,
                        Attribute = new[]
                        {
                            new Attribute {Name = MedComAttributes.ItSystemName, AttributeValue = "Sygdom.dk"},
                            new Attribute {Name = MedComAttributes.CareProviderId, AttributeValue = "30808460", NameFormat = "medcom:cvrnumber"},
                            new Attribute {Name = MedComAttributes.CareProviderName, AttributeValue = "Statens Serum Institut"}
                        }
                    }
                }
            };

            return certificate == null ? ass : SealUtilities.SignAssertion(ass, certificate);
        }

        public static Assertion MakeAssertion()
        {
            var vnow = DateTimeEx.UtcNowRound - TimeSpan.FromMinutes(5);

            return new Assertion
            {
                IssueInstant = vnow,
                id = "IDCard",
                Version = 2.0m,
                Issuer = "TESTSTS",
                Conditions = new Conditions
                {
                    NotBefore = vnow,
                    NotOnOrAfter = vnow + TimeSpan.FromHours(8)
                },
                Subject = new Subject
                {
                    NameID = new NameID
                    {
                        Format = "medcom:cprnumber",
                        Value = "2408631478"
                    },
                    SubjectConfirmation = new SubjectConfirmation
                    {
                        ConfirmationMethod = ConfirmationMethod.urnoasisnamestcSAML20cmholderofkey,
                        SubjectConfirmationData = new SubjectConfirmationData
                        {
                            Item = new KeyInfo
                            {
                                Item = "OCESSignature"
                            }
                        }
                    }
                },
                AttributeStatement = new[]
                {
                    new AttributeStatement
                    {
                        id = AttributeStatementID.IDCardData,
                        Attribute = new[]
                        {
                            new Attribute {Name = SosiAttributes.IDCardID, AttributeValue = Guid.NewGuid().ToString("D")},
                            new Attribute {Name = SosiAttributes.IDCardVersion, AttributeValue = "1.0.1"},
                            new Attribute {Name = SosiAttributes.IDCardType, AttributeValue = "user"},
                            new Attribute {Name = SosiAttributes.AuthenticationLevel, AttributeValue = "4"},
                            new Attribute {Name = SosiAttributes.OcesCertHash, AttributeValue = Global.cert.GetCertHashString()}
                        }
                    },
                    new AttributeStatement
                    {
                        id = AttributeStatementID.UserLog,
                        Attribute = new[]
                        {
                            new Attribute {Name = MedComAttributes.UserCivilRegistrationNumber, AttributeValue = "2408631478"},
                            new Attribute {Name = MedComAttributes.UserGivenName, AttributeValue = "Amaja Christiansen"},
                            new Attribute {Name = MedComAttributes.UserSurname, AttributeValue = "-"},
                            new Attribute {Name = MedComAttributes.UserEmailAddress, AttributeValue = "jso@trifork.com"},
                            new Attribute {Name = MedComAttributes.UserRole, AttributeValue = "5175"},
                            new Attribute {Name = MedComAttributes.UserAuthorizationCode, AttributeValue = "5GXFR"}
                        }
                    },
                    new AttributeStatement
                    {
                        id = AttributeStatementID.SystemLog,
                        Attribute = new[]
                        {
                            new Attribute {Name = MedComAttributes.ItSystemName, AttributeValue = "Sygdom.dk"},
                            new Attribute {Name = MedComAttributes.CareProviderId, AttributeValue = "25520041", NameFormat = "medcom:cvrnumber"},
                            new Attribute {Name = MedComAttributes.CareProviderName, AttributeValue = "TRIFORK SERVICES A/S // CVR:25520041"}
                        }
                    }
                }
            };
        }
    }
}